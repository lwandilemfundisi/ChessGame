"use client";

import { FormEvent, useCallback, useEffect, useMemo, useState } from "react";

type ChessPiece = {
  id: string | { value?: string };
  pieceName: string | { text?: string };
  pieceColor: string | { text?: string; yAxisDirectionIsUp?: boolean };
  xCoordinate: number;
  yCoordinate: number;
};

type Square = {
  xCoordinate: number;
  yCoordinate: number;
  chessPiece?: ChessPiece | null;
};

type ChessBoard = {
  id: string | { value?: string };
  blocks: Square[];
  whiteToMove?: boolean;
  gameStatus?: string;
};

const pieceGlyph: Record<string, string> = {
  king: "♚", queen: "♛", rook: "♜", bishop: "♝", knight: "♞", night: "♞", pawn: "♟",
};
const files = "abcdefgh";

function scalar(value: unknown): string {
  if (typeof value === "string" || typeof value === "number") return String(value);
  if (value && typeof value === "object") {
    const object = value as Record<string, unknown>;
    for (const key of ["value", "text", "id"]) if (object[key] != null) return scalar(object[key]);
  }
  return "";
}

function pieceName(piece: ChessPiece) { return scalar(piece.pieceName).toLowerCase(); }
function pieceId(piece: ChessPiece) { return scalar(piece.id); }
function isWhite(piece: ChessPiece) {
  if (piece.pieceColor && typeof piece.pieceColor === "object" && "yAxisDirectionIsUp" in piece.pieceColor)
    return Boolean(piece.pieceColor.yAxisDirectionIsUp);
  const color = scalar(piece.pieceColor).toLowerCase();
  return color.includes("white") || color === "w";
}
function boardId(board: ChessBoard) { return scalar(board.id); }
function boardFingerprint(board: ChessBoard) {
  return JSON.stringify([board.whiteToMove, board.gameStatus, board.blocks.map(({ xCoordinate, yCoordinate, chessPiece }) => [
    xCoordinate, yCoordinate, chessPiece ? [pieceId(chessPiece), pieceName(chessPiece), isWhite(chessPiece), chessPiece.xCoordinate, chessPiece.yCoordinate] : null,
  ])]);
}
function statusLabel(status?: string) {
  const labels: Record<string, string> = {
    InProgress: "Game in progress", Check: "Check", Checkmate: "Checkmate",
    Stalemate: "Draw · stalemate", DrawThreefoldRepetition: "Draw · repetition",
    DrawFiftyMove: "Draw · 50-move rule", DrawInsufficientMaterial: "Draw · insufficient material",
  };
  return status ? labels[status] ?? status : "Game in progress";
}

async function api<T>(path: string, init?: RequestInit): Promise<T> {
  console.info('logging');
  const response = await fetch(`/api/chess/${path}`, {
    ...init,
    headers: { ...(init?.body ? { "Content-Type": "application/json" } : {}), ...init?.headers },
    cache: "no-store",
  });
  const text = await response.text();
  let data: unknown;
  try { data = text ? JSON.parse(text) : null; } catch { data = text; }
  if (!response.ok) {
    const message = typeof data === "string" ? data : JSON.stringify(data);
    throw new Error(message || `Request failed (${response.status})`);
  }
  return data as T;
}

export default function Home() {
  const [board, setBoard] = useState<ChessBoard | null>(null);
  const [selected, setSelected] = useState<Square | null>(null);
  const [lastMove, setLastMove] = useState<{ from: string; to: string } | null>(null);
  const [history, setHistory] = useState<string[]>([]);
  const [boardIdInput, setBoardIdInput] = useState("");
  const [busy, setBusy] = useState(false);
  const [notice, setNotice] = useState("");
  const [promotion, setPromotion] = useState<{ square: Square; piece: ChessPiece } | null>(null);

  const boardSquares = useMemo(() => {
    const lookup = new Map<string, Square>();
    board?.blocks?.forEach((square) => lookup.set(`${square.xCoordinate},${square.yCoordinate}`, square));
    return Array.from({ length: 64 }, (_, index) => {
      const x = (index % 8) + 1;
      const y = 8 - Math.floor(index / 8);
      return lookup.get(`${x},${y}`) ?? { xCoordinate: x, yCoordinate: y };
    });
  }, [board]);

  useEffect(() => {
    const queryId = new URLSearchParams(window.location.search).get("game");
    const savedId = queryId || window.localStorage.getItem("chess-board-id");
    if (savedId) {
      setBoardIdInput(savedId);
      void loadGame(savedId, true);
    }
    // Load once when the page opens; subsequent loads are driven by user actions.
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  useEffect(() => {
    if (!board) return;
    const id = boardId(board);
    let active = true;
    const sync = async () => {
      if (document.visibilityState !== "visible") return;
      try {
        const latest = await api<ChessBoard>(`getboard?boardId=${encodeURIComponent(id)}`);
        if (active) setBoard((current) => current && boardFingerprint(current) === boardFingerprint(latest) ? current : latest);
      } catch {
        // Keep the current position on screen while the API is temporarily unavailable.
      }
    };
    const timer = window.setInterval(sync, 2500);
    return () => { active = false; window.clearInterval(timer); };
  }, [board ? boardId(board) : ""]);

  useEffect(() => { setSelected(null); }, [board?.whiteToMove]);

  const saveBoard = useCallback((next: ChessBoard) => {
    const id = boardId(next);
    setBoard(next);
    setBoardIdInput(id);
    if (id) window.localStorage.setItem("chess-board-id", id);
    setSelected(null);
    setPromotion(null);
  }, []);

  async function createGame() {
    setBusy(true); setNotice(""); setHistory([]); setLastMove(null);
    try {
      const next = await api<ChessBoard>("newgame", { method: "POST" });
      saveBoard(next);
      window.history.replaceState({}, "", `?game=${encodeURIComponent(boardId(next))}`);
      setNotice("A new game is ready. White moves first.");
    } catch (error) { setNotice(error instanceof Error ? error.message : "Could not start a game."); }
    finally { setBusy(false); }
  }

  async function loadGame(id: string, quiet = false) {
    if (!id.trim()) { setNotice("Enter a game ID to load a game."); return; }
    setBusy(true); setNotice("");
    try {
      const next = await api<ChessBoard>(`getboard?boardId=${encodeURIComponent(id.trim())}`);
      saveBoard(next);
      setHistory([]); setLastMove(null);
      window.history.replaceState({}, "", `?game=${encodeURIComponent(boardId(next))}`);
      if (!quiet) setNotice("Game loaded. Move history starts from this session.");
    } catch (error) { setNotice(error instanceof Error ? error.message : "Could not load that game."); }
    finally { setBusy(false); }
  }

  async function resetGame() {
    if (!board) return;
    setBusy(true); setNotice("");
    try {
      await api(`reset/${encodeURIComponent(boardId(board))}`, { method: "POST" });
      const next = await api<ChessBoard>(`getboard?boardId=${encodeURIComponent(boardId(board))}`);
      saveBoard(next); setHistory([]); setLastMove(null);
      setNotice("The board has been reset.");
    } catch (error) { setNotice(error instanceof Error ? error.message : "Could not reset this game."); }
    finally { setBusy(false); }
  }

  async function sendMove(from: Square, to: Square, promoteTo?: string) {
    const piece = from.chessPiece;
    if (!board || !piece) return;
    setBusy(true); setNotice("");
    try {
      await api("move", {
        method: "POST",
        body: JSON.stringify({
          boardId: boardId(board), chessPieceId: pieceId(piece),
          newXCoordinate: to.xCoordinate, newYCoordinate: to.yCoordinate,
          ...(promoteTo ? { promotionPieceName: promoteTo } : {}),
        }),
      });
      const next = await api<ChessBoard>(`getboard?boardId=${encodeURIComponent(boardId(board))}`);
      const fromName = `${files[from.xCoordinate - 1]}${from.yCoordinate}`;
      const toName = `${files[to.xCoordinate - 1]}${to.yCoordinate}`;
      saveBoard(next); setLastMove({ from: fromName, to: toName });
      setHistory((moves) => [...moves, `${moves.length + 1}. ${isWhite(piece) ? "White" : "Black"} · ${fromName} → ${toName}${promoteTo ? ` = ${promoteTo}` : ""}`]);
    } catch (error) { setNotice(error instanceof Error ? error.message : "That move could not be made."); setSelected(null); }
    finally { setBusy(false); }
  }

  function chooseSquare(square: Square) {
    if (busy || !board || (board.gameStatus !== "InProgress" && board.gameStatus !== "Check")) return;
    if (selected && selected.xCoordinate === square.xCoordinate && selected.yCoordinate === square.yCoordinate) { setSelected(null); return; }
    if (selected?.chessPiece) {
      if (square.chessPiece && isWhite(square.chessPiece) === isWhite(selected.chessPiece)) {
        setSelected(square); return;
      }
      const pawn = pieceName(selected.chessPiece) === "pawn";
      const reachesLastRank = pawn && ((isWhite(selected.chessPiece) && square.yCoordinate === 8) || (!isWhite(selected.chessPiece) && square.yCoordinate === 1));
      if (reachesLastRank) { setPromotion({ square, piece: selected.chessPiece }); return; }
      void sendMove(selected, square);
      return;
    }
    if (square.chessPiece && isWhite(square.chessPiece) === Boolean(board.whiteToMove)) setSelected(square);
  }

  const captured = useMemo(() => {
    const remaining = { white: new Map<string, number>(), black: new Map<string, number>() };
    board?.blocks?.forEach(({ chessPiece }) => {
      if (!chessPiece) return;
      const side = isWhite(chessPiece) ? "white" : "black";
      const name = pieceName(chessPiece);
      remaining[side].set(name, (remaining[side].get(name) ?? 0) + 1);
    });
    return { white: missingPieces(remaining.white), black: missingPieces(remaining.black) };
  }, [board]);

  function missingPieces(pieces: Map<string, number>) {
    const starting: Record<string, number> = { queen: 1, rook: 2, bishop: 2, night: 2, pawn: 8 };
    return Object.entries(starting).flatMap(([name, count]) => Array.from({ length: Math.max(0, count - (pieces.get(name) ?? (name === "night" ? pieces.get("knight") : 0) ?? 0)) }, () => name));
  }

  function submitGameId(event: FormEvent<HTMLFormElement>) { event.preventDefault(); void loadGame(boardIdInput); }

  const whiteToMove = board?.whiteToMove ?? true;
  const gameEnded = board && board.gameStatus !== "InProgress" && board.gameStatus !== "Check";

  return (
    <main className="app-shell">
      <header className="topbar">
        <a className="brand" href="/" aria-label="Chessroom home"><span className="brand-mark">♞</span><span>chessroom<span className="brand-dot">.</span></span></a>
        <div className="topbar-right"><span className="api-indicator"><i /> API connected through this app</span><a className="github-link" href="https://github.com/lwandilemfundisi/ChessGame" target="_blank" rel="noreferrer">Backend ↗</a></div>
      </header>

      <div className="workspace">
        <section className="play-area">
          <div className="section-heading"><div><p className="eyebrow">THE CLASSIC GAME</p><h1>Make your move.</h1></div><span className="round-tag"><i /> Live board</span></div>

          <div className="board-frame">
            <div className="player-strip opponent-strip">
              <div className="player-avatar black-avatar">♚</div><div className="player-copy"><strong>Black</strong><span>{board ? (whiteToMove ? "Waiting for White" : "To move") : "Opponent"}</span></div>
              <div className="captured-pieces">{captured.black.map((name, index) => <span key={`${name}-${index}`} className="captured-black">{pieceGlyph[name]}</span>)}</div>
            </div>

            {board ? <div className={`chessboard ${busy ? "board-busy" : ""}`} role="grid" aria-label="Chess board">
              {boardSquares.map((square) => {
                const file = files[square.xCoordinate - 1];
                const coord = `${file}${square.yCoordinate}`;
                const light = (square.xCoordinate + square.yCoordinate) % 2 === 0;
                const selectedHere = selected?.xCoordinate === square.xCoordinate && selected.yCoordinate === square.yCoordinate;
                const lastHere = lastMove?.from === coord || lastMove?.to === coord;
                const piece = square.chessPiece;
                return <button key={coord} role="gridcell" aria-label={`${coord}${piece ? ` ${isWhite(piece) ? "white" : "black"} ${pieceName(piece)}` : " empty"}`} className={`square ${light ? "light-square" : "dark-square"} ${selectedHere ? "selected-square" : ""} ${lastHere ? "last-square" : ""}`} onClick={() => chooseSquare(square)} disabled={busy || Boolean(gameEnded)}>
                  {square.xCoordinate === 1 && <span className="rank-coordinate">{square.yCoordinate}</span>}
                  {square.yCoordinate === 1 && <span className="file-coordinate">{file}</span>}
                  {piece && <span className={`piece ${isWhite(piece) ? "white-piece" : "black-piece"}`} aria-hidden="true">{pieceGlyph[pieceName(piece)] ?? "?"}</span>}
                  {selectedHere && <span className="selection-ring" />}
                </button>;
              })}
            </div> : <div className="board-empty"><span className="empty-knight">♞</span><strong>Your next game starts here.</strong><p>Start a new board or load a saved game by its ID.</p><button className="primary-button" onClick={createGame} disabled={busy}>{busy ? "Starting…" : "Start a new game"}<span>→</span></button></div>}

            <div className="player-strip home-strip">
              <div className="player-avatar white-avatar">♔</div><div className="player-copy"><strong>White</strong><span>{board ? (whiteToMove ? "To move" : "Waiting for Black") : "You"}</span></div>
              <div className="captured-pieces">{captured.white.map((name, index) => <span key={`${name}-${index}`} className="captured-white">{pieceGlyph[name]}</span>)}</div>
            </div>
          </div>
          <div className="board-caption"><span>♙ &nbsp; Click a piece, then a destination square</span><span>Files A–H &nbsp;·&nbsp; Ranks 1–8</span></div>
        </section>

        <aside className="side-panel">
          <section className="game-card">
            <div className="card-heading"><div><p className="eyebrow">GAME ROOM</p><h2>Match details</h2></div><span className="card-icon">⌘</span></div>
            <div className={`turn-banner ${gameEnded ? "ended-banner" : ""}`}><span className="turn-piece">{gameEnded ? "◇" : whiteToMove ? "♙" : "♟"}</span><div><strong>{board ? statusLabel(board.gameStatus) : "No active game"}</strong><small>{board ? (gameEnded ? "This game is complete" : `${whiteToMove ? "White" : "Black"} to move`) : "Create or load a board"}</small></div><span className="turn-arrow">{board && !gameEnded ? "↗" : ""}</span></div>
            <div className="meta-row"><span>Game ID</span><strong className="game-id">{board ? boardId(board) : "—"}</strong></div>
            {board && <button className="share-button" onClick={async () => { await navigator.clipboard.writeText(`${window.location.origin}/?game=${encodeURIComponent(boardId(board))}`); setNotice("Game link copied to clipboard."); }}>↗ <span>Copy game link</span></button>}
            <div className="card-divider" />
            <button className="primary-button full-button" onClick={createGame} disabled={busy}>{board ? "New game" : "Start a new game"}<span>＋</span></button>
            <button className="secondary-button full-button" onClick={resetGame} disabled={busy || !board}>Reset this board<span>↺</span></button>
          </section>

          <section className="load-card"><div className="card-heading compact-heading"><div><p className="eyebrow">RETURN TO A MATCH</p><h2>Load a game</h2></div><span className="card-icon pale-icon">⌕</span></div>
            <form className="load-form" onSubmit={submitGameId}><label htmlFor="game-id">Game ID</label><div className="input-row"><input id="game-id" value={boardIdInput} onChange={(event) => setBoardIdInput(event.target.value)} placeholder="Paste a game ID…" /><button type="submit" disabled={busy}>Load</button></div></form>
            <p className="helper-copy">Share the same game ID with your opponent to play on this board.</p>
          </section>

          <section className="history-card"><div className="card-heading compact-heading"><div><p className="eyebrow">THIS SESSION</p><h2>Move list</h2></div><span className="move-count">{history.length}</span></div>
            {history.length ? <ol className="move-list">{history.slice().reverse().map((move, index) => <li key={`${move}-${index}`}>{move}</li>)}</ol> : <div className="history-empty"><span>↳</span><p>Your moves will appear here as you play.</p></div>}
            {board && <p className="history-note">Move list begins when this page session starts.</p>}
          </section>
          {notice && <div className={`notice ${notice.toLowerCase().includes("could not") || notice.toLowerCase().includes("failed") ? "notice-error" : ""}`} role="status"><span>{notice.toLowerCase().includes("could not") ? "!" : "i"}</span><p>{notice}</p><button aria-label="Dismiss message" onClick={() => setNotice("")}>×</button></div>}
          <footer>Powered by <strong>ChessGame API</strong><span>·</span> Standard chess rules</footer>
        </aside>
      </div>

      {promotion && <div className="modal-backdrop" role="presentation"><section className="promotion-modal" role="dialog" aria-modal="true" aria-labelledby="promotion-title"><p className="eyebrow">PAWN PROMOTION</p><h2 id="promotion-title">Choose your piece</h2><p>Your pawn reached the other side of the board.</p><div className="promotion-options">{["Queen", "Rook", "Bishop", "Knight"].map((name) => <button key={name} onClick={() => void sendMove(selected!, promotion.square, name)}><span>{pieceGlyph[name.toLowerCase()]}</span>{name}</button>)}</div><button className="cancel-button" onClick={() => setPromotion(null)}>Cancel</button></section></div>}
    </main>
  );
}
