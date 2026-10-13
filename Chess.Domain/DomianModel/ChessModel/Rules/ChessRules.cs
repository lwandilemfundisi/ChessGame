using Chess.Domain.DomianModel.ChessModel.Entities;
using Chess.Domain.DomianModel.ChessModel.ValueObjects;
using Chess.Domain.DomianModel.ChessModel.ValueObjects.LookupValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess.Domain.DomianModel.ChessModel.Rules
{
    /// <summary>Legal move and position rules for standard chess on an 8 by 8 board.</summary>
    public static class ChessRules
    {
        public const string InProgress = "InProgress";
        public const string Check = "Check";
        public const string Checkmate = "Checkmate";
        public const string Stalemate = "Stalemate";
        public const string DrawThreefoldRepetition = "DrawThreefoldRepetition";
        public const string DrawFiftyMove = "DrawFiftyMove";
        public const string DrawInsufficientMaterial = "DrawInsufficientMaterial";

        public static string PositionKey(IReadOnlyCollection<Block> board, bool whiteToMove, uint enPassantX = 0, uint enPassantY = 0)
        {
            var placement = string.Join(";", board.Where(b => b.ChessPiece != null)
                .OrderBy(b => b.YCoordinate).ThenBy(b => b.XCoordinate)
                .Select(b => $"{b.XCoordinate},{b.YCoordinate},{Name(b.ChessPiece)},{(IsWhite(b.ChessPiece) ? 'w' : 'b')}") );
            var rights = new System.Text.StringBuilder();
            foreach (var white in new[] { true, false })
            {
                var rank = white ? 1u : 8u;
                var king = At(board, 5, rank)?.ChessPiece;
                var canCastle = king != null && Name(king) == "King" && IsWhite(king) == white && !king.HasMovedSinceStart;
                rights.Append(canCastle && At(board, 8, rank)?.ChessPiece is ChessPiece kr && Name(kr) == "Rook" && IsWhite(kr) == white && !kr.HasMovedSinceStart ? 'K' : '-');
                rights.Append(canCastle && At(board, 1, rank)?.ChessPiece is ChessPiece qr && Name(qr) == "Rook" && IsWhite(qr) == white && !qr.HasMovedSinceStart ? 'Q' : '-');
            }
            var ep = enPassantX != 0 && board.Any(b => b.XCoordinate != enPassantX && Math.Abs((int)b.XCoordinate - (int)enPassantX) == 1 &&
                b.YCoordinate == (whiteToMove ? enPassantY - 1 : enPassantY + 1) && b.ChessPiece != null && Name(b.ChessPiece) == "Pawn" && IsWhite(b.ChessPiece) == whiteToMove)
                ? $"{enPassantX},{enPassantY}" : "-";
            return $"{placement}|{(whiteToMove ? 'w' : 'b')}|{rights}|{ep}";
        }

        public static bool IsInsufficientMaterial(IReadOnlyCollection<Block> board)
        {
            var pieces = board.Where(b => b.ChessPiece != null && Name(b.ChessPiece) != "King").ToList();
            if (pieces.Count == 0) return true;
            if (pieces.Any(b => Name(b.ChessPiece) == "Pawn" || Name(b.ChessPiece) == "Rook" || Name(b.ChessPiece) == "Queen")) return false;
            if (pieces.Count == 1) return true; // lone bishop or knight cannot force mate
            if (pieces.All(b => Name(b.ChessPiece) == "Bishop"))
            {
                var squareColors = pieces.Select(b => (b.XCoordinate + b.YCoordinate) % 2).Distinct().Count();
                return squareColors == 1;
            }
            return false;
        }

        public static bool IsFiftyMoveDraw(int halfMoveClock) => halfMoveClock >= 100;

        public static string ValidateMove(IReadOnlyCollection<Block> board, Move move, uint enPassantX = 0, uint enPassantY = 0)
        {
            var origin = board.FirstOrDefault(b => b.ChessPiece?.Id == move.PieceId);
            if (origin == null) return "The selected piece is not on this board.";
            if (!Inside(move.NewXCoordinate, move.NewYCoordinate)) return "Destination must be inside the board.";

            var destination = At(board, move.NewXCoordinate, move.NewYCoordinate);
            if (destination == null || (origin.XCoordinate == move.NewXCoordinate && origin.YCoordinate == move.NewYCoordinate))
                return "A piece must move to a different square on the board.";

            var piece = origin.ChessPiece;
            var target = destination.ChessPiece;
            if (target != null && SameSide(piece, target)) return "You cannot capture your own piece.";
            if (target != null && Name(target) == "King") return "A king is never captured; the game ends at checkmate.";
            if (Name(piece) != "Pawn" && move.PromotionPieceName != null)
                return "Only a pawn can be promoted.";

            var dx = (int)move.NewXCoordinate - (int)origin.XCoordinate;
            var dy = (int)move.NewYCoordinate - (int)origin.YCoordinate;
            var ax = Math.Abs(dx);
            var ay = Math.Abs(dy);
            var name = Name(piece);
            var enPassant = false;
            var castling = false;
            var legal = false;

            switch (name)
            {
                case "Pawn":
                {
                    var forward = IsWhite(piece) ? 1 : -1;
                    var homeRank = IsWhite(piece) ? 2u : 7u;
                    if (dx == 0 && target == null && dy == forward) legal = true;
                    else if (dx == 0 && target == null && origin.YCoordinate == homeRank && !piece.HasMovedSinceStart && dy == 2 * forward)
                        legal = PathClear(board, (int)origin.XCoordinate, (int)origin.YCoordinate, (int)move.NewXCoordinate, (int)move.NewYCoordinate);
                    else if (ax == 1 && dy == forward && target != null) legal = true;
                    else if (ax == 1 && dy == forward && target == null &&
                        move.NewXCoordinate == enPassantX && move.NewYCoordinate == enPassantY)
                    {
                        var adjacent = At(board, move.NewXCoordinate, origin.YCoordinate)?.ChessPiece;
                        enPassant = adjacent != null && Name(adjacent) == "Pawn" && !SameSide(piece, adjacent);
                        legal = enPassant;
                    }
                    var promotes = (IsWhite(piece) && move.NewYCoordinate == 8) || (!IsWhite(piece) && move.NewYCoordinate == 1);
                    if (promotes && (move.PromotionPieceName != null && !IsPromotionPiece(move.PromotionPieceName)))
                        return "A pawn can promote only to a queen, rook, bishop, or knight.";
                    if (!promotes && move.PromotionPieceName != null)
                        return "Promotion can only be selected when a pawn reaches its final rank.";
                    break;
                }
                case "Night":
                case "Knight":
                    legal = (ax == 1 && ay == 2) || (ax == 2 && ay == 1);
                    break;
                case "Bishop":
                    legal = ax == ay && PathClear(board, (int)origin.XCoordinate, (int)origin.YCoordinate, (int)move.NewXCoordinate, (int)move.NewYCoordinate);
                    break;
                case "Rook":
                    legal = (dx == 0 || dy == 0) && PathClear(board, (int)origin.XCoordinate, (int)origin.YCoordinate, (int)move.NewXCoordinate, (int)move.NewYCoordinate);
                    break;
                case "Queen":
                    legal = (ax == ay || dx == 0 || dy == 0) && PathClear(board, (int)origin.XCoordinate, (int)origin.YCoordinate, (int)move.NewXCoordinate, (int)move.NewYCoordinate);
                    break;
                case "King":
                {
                    legal = Math.Max(ax, ay) == 1;
                    if (!legal && ay == 0 && ax == 2)
                    {
                        var error = ValidateCastle(board, origin, move.NewXCoordinate, out var rookBlock);
                        if (error != null) return error;
                        castling = rookBlock != null;
                        legal = castling;
                    }
                    break;
                }
                default:
                    return "Unknown chess piece.";
            }

            if (!legal) return "Move is not legal for a " + name + ".";
            if (!LeavesKingSafe(board, origin, destination, piece, enPassant))
                return "This move would leave your king in check.";
            if (castling)
            {
                var transitX = (uint)((int)origin.XCoordinate + Math.Sign(dx));
                if (IsKingAttackedAfterMove(board, origin, transitX, origin.YCoordinate))
                    return "A king cannot castle through check.";
            }
            return null;
        }

        public static bool IsKingInCheck(IReadOnlyCollection<Block> board, bool white)
        {
            var king = board.FirstOrDefault(b => b.ChessPiece != null && Name(b.ChessPiece) == "King" && IsWhite(b.ChessPiece) == white);
            return king != null && IsSquareAttacked(board, (int)king.XCoordinate, (int)king.YCoordinate, !white);
        }

        public static bool HasLegalMove(IReadOnlyCollection<Block> board, bool white, uint enPassantX = 0, uint enPassantY = 0)
        {
            foreach (var block in board.Where(b => b.ChessPiece != null && IsWhite(b.ChessPiece) == white))
            {
                for (uint x = 1; x <= 8; x++)
                    for (uint y = 1; y <= 8; y++)
                    {
                        var move = new Move(block.ChessPiece.Id, x, y);
                        if (ValidateMove(board, move, enPassantX, enPassantY) == null) return true;
                    }
            }
            return false;
        }

        public static string GetPositionStatus(IReadOnlyCollection<Block> board, bool sideToMoveIsWhite, uint enPassantX = 0, uint enPassantY = 0)
        {
            // Reduced boards are supported for isolated piece-rule scenarios.
            if (!board.Any(b => b.ChessPiece != null && Name(b.ChessPiece) == "King" && IsWhite(b.ChessPiece)) ||
                !board.Any(b => b.ChessPiece != null && Name(b.ChessPiece) == "King" && !IsWhite(b.ChessPiece)))
                return InProgress;

            var check = IsKingInCheck(board, sideToMoveIsWhite);
            if (HasLegalMove(board, sideToMoveIsWhite, enPassantX, enPassantY)) return check ? Check : InProgress;
            return check ? Checkmate : Stalemate;
        }

        public static bool IsWhite(ChessPiece piece) => piece.PieceColor.YAxisDirectionIsUp;

        public static string Name(ChessPiece piece) => piece.PieceName?.Text;

        private static string ValidateCastle(IReadOnlyCollection<Block> board, Block kingBlock, uint destinationX, out Block rookBlock)
        {
            rookBlock = null;
            var king = kingBlock.ChessPiece;
            var homeRank = IsWhite(king) ? 1u : 8u;
            if (king.HasMovedSinceStart || kingBlock.YCoordinate != homeRank || kingBlock.XCoordinate != 5)
                return "Castling requires the unmoved king on its starting square.";
            var kingSide = destinationX > kingBlock.XCoordinate;
            if (destinationX != (kingSide ? 7u : 3u)) return "The king must castle to the standard destination square.";
            var rookX = kingSide ? 8u : 1u;
            rookBlock = At(board, rookX, homeRank);
            var rook = rookBlock?.ChessPiece;
            if (rook == null || Name(rook) != "Rook" || !SameSide(king, rook) || rook.HasMovedSinceStart)
            {
                rookBlock = null;
                return "Castling requires an unmoved rook on that side.";
            }
            var direction = kingSide ? 1 : -1;
            for (var x = (int)kingBlock.XCoordinate + direction; x != (int)rookX; x += direction)
                if (At(board, (uint)x, homeRank)?.ChessPiece != null) return "Castling path must be clear.";
            if (IsSquareAttacked(board, (int)kingBlock.XCoordinate, (int)kingBlock.YCoordinate, !IsWhite(king)))
                return "A king cannot castle out of check.";
            return null;
        }

        private static bool LeavesKingSafe(IReadOnlyCollection<Block> board, Block origin, Block destination, ChessPiece movingPiece, bool enPassant)
        {
            var pieces = PositionAfterMove(board, origin, destination, movingPiece, enPassant);
            var king = pieces.FirstOrDefault(p => Name(p.Piece) == "King" && IsWhite(p.Piece) == IsWhite(movingPiece));
            return king == null || !IsAttacked(pieces, king.X, king.Y, !IsWhite(movingPiece));
        }

        private static bool IsKingAttackedAfterMove(IReadOnlyCollection<Block> board, Block origin, uint x, uint y)
        {
            var pieces = board.Where(b => b.ChessPiece != null && b != origin)
                .Select(b => new PiecePosition((int)b.XCoordinate, (int)b.YCoordinate, b.ChessPiece)).ToList();
            pieces.Add(new PiecePosition((int)x, (int)y, origin.ChessPiece));
            return IsAttacked(pieces, (int)x, (int)y, !IsWhite(origin.ChessPiece));
        }

        private static List<PiecePosition> PositionAfterMove(IReadOnlyCollection<Block> board, Block origin, Block destination, ChessPiece movingPiece, bool enPassant)
        {
            var pieces = board.Where(b => b.ChessPiece != null && b != origin && b != destination)
                .Select(b => new PiecePosition((int)b.XCoordinate, (int)b.YCoordinate, b.ChessPiece)).ToList();
            if (enPassant)
                pieces.RemoveAll(p => p.X == (int)destination.XCoordinate && p.Y == (int)origin.YCoordinate);
            pieces.Add(new PiecePosition((int)destination.XCoordinate, (int)destination.YCoordinate, movingPiece));
            return pieces;
        }

        private static bool IsSquareAttacked(IReadOnlyCollection<Block> board, int x, int y, bool byWhite)
        {
            var pieces = board.Where(b => b.ChessPiece != null)
                .Select(b => new PiecePosition((int)b.XCoordinate, (int)b.YCoordinate, b.ChessPiece)).ToList();
            return IsAttacked(pieces, x, y, byWhite);
        }

        private static bool IsAttacked(IEnumerable<PiecePosition> pieces, int x, int y, bool byWhite)
        {
            foreach (var p in pieces.Where(p => IsWhite(p.Piece) == byWhite))
            {
                var dx = x - p.X;
                var dy = y - p.Y;
                var ax = Math.Abs(dx);
                var ay = Math.Abs(dy);
                var name = Name(p.Piece);
                if (name == "Pawn" && ax == 1 && dy == (byWhite ? 1 : -1)) return true;
                if ((name == "Knight" || name == "Night") && ((ax == 1 && ay == 2) || (ax == 2 && ay == 1))) return true;
                if (name == "King" && Math.Max(ax, ay) == 1) return true;
                var line = name == "Bishop" && ax == ay || name == "Rook" && (dx == 0 || dy == 0) ||
                    name == "Queen" && (ax == ay || dx == 0 || dy == 0);
                if (line && PathClear(pieces, p.X, p.Y, x, y)) return true;
            }
            return false;
        }

        private static bool PathClear(IEnumerable<PiecePosition> pieces, int fromX, int fromY, int toX, int toY)
        {
            var dx = Math.Sign(toX - fromX);
            var dy = Math.Sign(toY - fromY);
            var x = fromX + dx;
            var y = fromY + dy;
            while (x != toX || y != toY)
            {
                if (pieces.Any(p => p.X == x && p.Y == y)) return false;
                x += dx;
                y += dy;
            }
            return true;
        }

        private static bool PathClear(IReadOnlyCollection<Block> board, int fromX, int fromY, int toX, int toY)
        {
            return PathClear(board.Where(b => b.ChessPiece != null)
                .Select(b => new PiecePosition((int)b.XCoordinate, (int)b.YCoordinate, b.ChessPiece)), fromX, fromY, toX, toY);
        }

        private static bool SameSide(ChessPiece a, ChessPiece b) => IsWhite(a) == IsWhite(b);
        private static bool Inside(uint x, uint y) => x >= 1 && x <= 8 && y >= 1 && y <= 8;
        private static Block At(IReadOnlyCollection<Block> board, uint x, uint y) => board.FirstOrDefault(b => b.XCoordinate == x && b.YCoordinate == y);
        private static bool IsPromotionPiece(string name)
        {
            switch (name?.ToLowerInvariant())
            {
                case "queen":
                case "rook":
                case "bishop":
                case "knight":
                case "night": return true;
                default: return false;
            }
        }

        private sealed class PiecePosition
        {
            public PiecePosition(int x, int y, ChessPiece piece) { X = x; Y = y; Piece = piece; }
            public int X { get; }
            public int Y { get; }
            public ChessPiece Piece { get; }
        }
    }
}
