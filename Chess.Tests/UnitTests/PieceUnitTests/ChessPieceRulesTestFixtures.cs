using Chess.Domain;
using Chess.Domain.DomianModel.ChessModel;
using Chess.Domain.DomianModel.ChessModel.Commands;
using Chess.Domain.DomianModel.ChessModel.Entities;
using Chess.Domain.DomianModel.ChessModel.Queries;
using Chess.Domain.DomianModel.ChessModel.ValueObjects;
using Chess.Domain.DomianModel.ChessModel.ValueObjects.LookupValueObjects;
using Chess.Domain.Extensions;
using Chess.Persistence.Extensions;
using Chess.Tests.Context;
using Microservice.Framework.Domain.Commands;
using Microservice.Framework.Domain.Exceptions;
using Microservice.Framework.Domain.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Chess.Tests.UnitTests.PieceUnitTests
{
    [TestClass]
    public class ChessPieceRulesTestFixtures
    {
        private IServiceProvider _provider;
        private ICommandBus _commandBus;
        private IQueryProcessor _queryProcessor;

        [TestInitialize]
        public void Startup()
        {
            var services = new ServiceCollection();
            services.AddLogging(l => l.AddConsole())
                .ConfigureChessDomain()
                .ConfigureChessPersistence<ChessInMemoryContext, TestChessContextProvider>();
            _provider = services.BuildServiceProvider();
            _commandBus = _provider.GetRequiredService<ICommandBus>();
            _queryProcessor = _provider.GetRequiredService<IQueryProcessor>();
        }

        [TestCleanup]
        public void Cleanup() => (_provider as IDisposable)?.Dispose();

        [TestMethod]
        public async Task KnightCanLeapOverPieces()
        {
            var board = await CreateBoard(Position(
                Piece(PieceNames.Of().Night, Colors.Of().White, 2, 1),
                Piece(PieceNames.Of().Pawn, Colors.Of().White, 3, 2),
                Piece(PieceNames.Of().King, Colors.Of().White, 5, 1),
                Piece(PieceNames.Of().King, Colors.Of().Black, 5, 8)));

            board = await Move(board.Id, 2, 1, 3, 3);

            Assert.AreEqual("Night", PieceAt(board, 3, 3).PieceName.Text);
        }

        [TestMethod]
        [ExpectedException(typeof(DomainError))]
        public async Task KnightRejectsNonLShapedMove()
        {
            var board = await CreateBoard(Position(
                Piece(PieceNames.Of().Night, Colors.Of().White, 2, 1),
                Piece(PieceNames.Of().King, Colors.Of().White, 5, 1),
                Piece(PieceNames.Of().King, Colors.Of().Black, 5, 8)));
            await Move(board.Id, 2, 1, 3, 2);
        }

        [TestMethod]
        public async Task BishopMovesDiagonally()
        {
            var board = await CreateBoard(Position(
                Piece(PieceNames.Of().Bishop, Colors.Of().White, 3, 1),
                Piece(PieceNames.Of().King, Colors.Of().White, 5, 1),
                Piece(PieceNames.Of().King, Colors.Of().Black, 5, 8)));
            board = await Move(board.Id, 3, 1, 6, 4);
            Assert.AreEqual("Bishop", PieceAt(board, 6, 4).PieceName.Text);
        }

        [TestMethod]
        [ExpectedException(typeof(DomainError))]
        public async Task BishopCannotJumpOverPieces()
        {
            var board = await CreateBoard(Position(
                Piece(PieceNames.Of().Bishop, Colors.Of().White, 3, 1),
                Piece(PieceNames.Of().Pawn, Colors.Of().White, 4, 2),
                Piece(PieceNames.Of().King, Colors.Of().White, 5, 1),
                Piece(PieceNames.Of().King, Colors.Of().Black, 5, 8)));
            await Move(board.Id, 3, 1, 6, 4);
        }

        [TestMethod]
        public async Task RookMovesVerticallyAndCapturesOpponent()
        {
            var board = await CreateBoard(Position(
                Piece(PieceNames.Of().Rook, Colors.Of().White, 1, 1),
                Piece(PieceNames.Of().Pawn, Colors.Of().Black, 1, 4),
                Piece(PieceNames.Of().King, Colors.Of().White, 5, 1),
                Piece(PieceNames.Of().King, Colors.Of().Black, 5, 8)));
            board = await Move(board.Id, 1, 1, 1, 4);
            Assert.AreEqual("Rook", PieceAt(board, 1, 4).PieceName.Text);
        }

        [TestMethod]
        [ExpectedException(typeof(DomainError))]
        public async Task RookRejectsDiagonalMove()
        {
            var board = await CreateBoard(Position(
                Piece(PieceNames.Of().Rook, Colors.Of().White, 1, 1),
                Piece(PieceNames.Of().King, Colors.Of().White, 5, 1),
                Piece(PieceNames.Of().King, Colors.Of().Black, 5, 8)));
            await Move(board.Id, 1, 1, 3, 3);
        }

        [TestMethod]
        public async Task QueenMovesDiagonallyAndOrthogonally()
        {
            var board = await CreateBoard(Position(
                Piece(PieceNames.Of().Queen, Colors.Of().White, 4, 1),
                Piece(PieceNames.Of().King, Colors.Of().White, 5, 1),
                Piece(PieceNames.Of().King, Colors.Of().Black, 5, 8)));
            board = await Move(board.Id, 4, 1, 7, 4);
            board = await Move(board.Id, 5, 8, 5, 7);
            board = await Move(board.Id, 7, 4, 7, 8);
            Assert.AreEqual("Queen", PieceAt(board, 7, 8).PieceName.Text);
        }

        [TestMethod]
        public async Task KingMovesOneSquareAndCannotEnterCheck()
        {
            var board = await CreateBoard(Position(
                Piece(PieceNames.Of().King, Colors.Of().White, 5, 1),
                Piece(PieceNames.Of().King, Colors.Of().Black, 5, 8),
                Piece(PieceNames.Of().Rook, Colors.Of().Black, 6, 8)));
            board = await Move(board.Id, 5, 1, 5, 2);
            Assert.AreEqual("King", PieceAt(board, 5, 2).PieceName.Text);
        }

        [TestMethod]
        [ExpectedException(typeof(DomainError))]
        public async Task KingCannotMoveOntoARookAttackedSquare()
        {
            var board = await CreateBoard(Position(
                Piece(PieceNames.Of().King, Colors.Of().White, 4, 1),
                Piece(PieceNames.Of().King, Colors.Of().Black, 1, 8),
                Piece(PieceNames.Of().Rook, Colors.Of().Black, 5, 8)));
            await Move(board.Id, 4, 1, 5, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(DomainError))]
        public async Task PinnedPieceCannotExposeItsKing()
        {
            var board = await CreateBoard(Position(
                Piece(PieceNames.Of().King, Colors.Of().White, 5, 1),
                Piece(PieceNames.Of().Rook, Colors.Of().White, 5, 2),
                Piece(PieceNames.Of().King, Colors.Of().Black, 1, 8),
                Piece(PieceNames.Of().Rook, Colors.Of().Black, 5, 8)));
            await Move(board.Id, 5, 2, 6, 2);
        }

        [TestMethod]
        public async Task KingAndRookCastleTogether()
        {
            var board = await CreateBoard(Position(
                Piece(PieceNames.Of().King, Colors.Of().White, 5, 1),
                Piece(PieceNames.Of().Rook, Colors.Of().White, 8, 1),
                Piece(PieceNames.Of().King, Colors.Of().Black, 5, 8)));
            board = await Move(board.Id, 5, 1, 7, 1);
            Assert.AreEqual("King", PieceAt(board, 7, 1).PieceName.Text);
            Assert.AreEqual("Rook", PieceAt(board, 6, 1).PieceName.Text);
            Assert.IsTrue(PieceAt(board, 6, 1).HasMovedSinceStart);
        }

        [TestMethod]
        public async Task KingAndQueenSideRookCastleTogether()
        {
            var board = await CreateBoard(Position(
                Piece(PieceNames.Of().King, Colors.Of().White, 5, 1),
                Piece(PieceNames.Of().Rook, Colors.Of().White, 1, 1),
                Piece(PieceNames.Of().King, Colors.Of().Black, 5, 8)));
            board = await Move(board.Id, 5, 1, 3, 1);
            Assert.AreEqual("King", PieceAt(board, 3, 1).PieceName.Text);
            Assert.AreEqual("Rook", PieceAt(board, 4, 1).PieceName.Text);
        }

        [TestMethod]
        [ExpectedException(typeof(DomainError))]
        public async Task KingCannotMoveTwoSquaresWithoutCastlingRights()
        {
            var board = await CreateBoard(Position(
                Piece(PieceNames.Of().King, Colors.Of().White, 5, 1),
                Piece(PieceNames.Of().King, Colors.Of().Black, 5, 8)));
            await Move(board.Id, 5, 1, 7, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(DomainError))]
        public async Task KingCannotCastleThroughAnAttackedSquare()
        {
            var board = await CreateBoard(Position(
                Piece(PieceNames.Of().King, Colors.Of().White, 5, 1),
                Piece(PieceNames.Of().Rook, Colors.Of().White, 8, 1),
                Piece(PieceNames.Of().King, Colors.Of().Black, 1, 8),
                Piece(PieceNames.Of().Rook, Colors.Of().Black, 6, 8)));
            await Move(board.Id, 5, 1, 7, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(DomainError))]
        public async Task KingCannotCastleAfterMovingAwayAndReturning()
        {
            var board = await CreateBoard(Position(
                Piece(PieceNames.Of().King, Colors.Of().White, 5, 1),
                Piece(PieceNames.Of().Rook, Colors.Of().White, 8, 1),
                Piece(PieceNames.Of().King, Colors.Of().Black, 1, 8)));
            board = await Move(board.Id, 5, 1, 5, 2);
            board = await Move(board.Id, 1, 8, 1, 7);
            board = await Move(board.Id, 5, 2, 5, 1);
            board = await Move(board.Id, 1, 7, 1, 8);
            await Move(board.Id, 5, 1, 7, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(DomainError))]
        public async Task RookCannotCastleAfterMovingAwayAndReturning()
        {
            var board = await CreateBoard(Position(
                Piece(PieceNames.Of().King, Colors.Of().White, 5, 1),
                Piece(PieceNames.Of().Rook, Colors.Of().White, 8, 1),
                Piece(PieceNames.Of().King, Colors.Of().Black, 1, 8)));
            board = await Move(board.Id, 8, 1, 8, 2);
            board = await Move(board.Id, 1, 8, 1, 7);
            board = await Move(board.Id, 8, 2, 8, 1);
            board = await Move(board.Id, 1, 7, 1, 8);
            await Move(board.Id, 5, 1, 7, 1);
        }

        [TestMethod]
        public async Task EnPassantCapturesThePawnThatJustAdvancedTwoSquares()
        {
            var board = await CreateBoard(Position(
                Piece(PieceNames.Of().King, Colors.Of().White, 1, 1),
                Piece(PieceNames.Of().King, Colors.Of().Black, 8, 8),
                Piece(PieceNames.Of().Pawn, Colors.Of().White, 5, 5),
                Piece(PieceNames.Of().Pawn, Colors.Of().Black, 4, 7)));
            board = await Move(board.Id, 1, 1, 1, 2);
            board = await Move(board.Id, 4, 7, 4, 5);
            board = await Move(board.Id, 5, 5, 4, 6);
            Assert.AreEqual("Pawn", PieceAt(board, 4, 6).PieceName.Text);
            Assert.IsNull(PieceAt(board, 4, 5));
        }

        [TestMethod]
        [ExpectedException(typeof(DomainError))]
        public async Task EnPassantRightExpiresAfterOneTurn()
        {
            var board = await CreateBoard(Position(
                Piece(PieceNames.Of().King, Colors.Of().White, 1, 1),
                Piece(PieceNames.Of().King, Colors.Of().Black, 8, 8),
                Piece(PieceNames.Of().Pawn, Colors.Of().White, 5, 5),
                Piece(PieceNames.Of().Pawn, Colors.Of().Black, 4, 7)));
            board = await Move(board.Id, 1, 1, 1, 2);
            board = await Move(board.Id, 4, 7, 4, 5);
            board = await Move(board.Id, 1, 2, 1, 3);
            board = await Move(board.Id, 8, 8, 8, 7);
            await Move(board.Id, 5, 5, 4, 6);
        }

        [TestMethod]
        [ExpectedException(typeof(DomainError))]
        public async Task EnPassantCannotExposeKingToDiscoveredRookCheck()
        {
            var board = await CreateBoard(Position(
                Piece(PieceNames.Of().King, Colors.Of().White, 5, 5),
                Piece(PieceNames.Of().King, Colors.Of().Black, 1, 8),
                Piece(PieceNames.Of().Night, Colors.Of().White, 1, 1),
                Piece(PieceNames.Of().Pawn, Colors.Of().White, 6, 5),
                Piece(PieceNames.Of().Pawn, Colors.Of().Black, 7, 7),
                Piece(PieceNames.Of().Rook, Colors.Of().Black, 8, 5)));
            board = await Move(board.Id, 1, 1, 2, 3);
            board = await Move(board.Id, 7, 7, 7, 5);
            await Move(board.Id, 6, 5, 7, 6);
        }

        [TestMethod]
        public async Task PawnCanPromoteToChosenKnight()
        {
            var board = await CreateBoard(Position(
                Piece(PieceNames.Of().King, Colors.Of().White, 8, 1),
                Piece(PieceNames.Of().King, Colors.Of().Black, 8, 8),
                Piece(PieceNames.Of().Pawn, Colors.Of().White, 1, 7)));
            board = await Move(board.Id, 1, 7, 1, 8, "Knight");
            Assert.AreEqual("Night", PieceAt(board, 1, 8).PieceName.Text);
        }

        [TestMethod]
        [ExpectedException(typeof(DomainError))]
        public async Task PawnRejectsInvalidPromotionPiece()
        {
            var board = await CreateBoard(Position(
                Piece(PieceNames.Of().King, Colors.Of().White, 8, 1),
                Piece(PieceNames.Of().King, Colors.Of().Black, 8, 8),
                Piece(PieceNames.Of().Pawn, Colors.Of().White, 1, 7)));
            await Move(board.Id, 1, 7, 1, 8, "King");
        }

        [TestMethod]
        public async Task CheckmateEndsTheGame()
        {
            var board = await CreateBoard(ChessExtensions.BuildBoard().PlaceAllPieces());
            board = await Move(board.Id, 6, 2, 6, 3); // f3
            board = await Move(board.Id, 5, 7, 5, 5); // ...e5
            board = await Move(board.Id, 7, 2, 7, 4); // g4
            board = await Move(board.Id, 4, 8, 8, 4); // ...Qh4#
            Assert.AreEqual("Checkmate", board.GameStatus);
        }

        [TestMethod]
        public async Task StalemateIsReported()
        {
            var board = await CreateBoard(Position(
                Piece(PieceNames.Of().King, Colors.Of().White, 3, 6),
                Piece(PieceNames.Of().Queen, Colors.Of().White, 2, 5),
                Piece(PieceNames.Of().King, Colors.Of().Black, 1, 8)));
            board = await Move(board.Id, 2, 5, 2, 6);
            Assert.AreEqual("Stalemate", board.GameStatus);
        }

        [TestMethod]
        public async Task ThirdOccurrenceOfPositionEndsInThreefoldDraw()
        {
            var board = await CreateBoard(Position(
                Piece(PieceNames.Of().King, Colors.Of().White, 5, 1),
                Piece(PieceNames.Of().King, Colors.Of().Black, 5, 8),
                Piece(PieceNames.Of().Night, Colors.Of().White, 2, 1),
                Piece(PieceNames.Of().Night, Colors.Of().Black, 2, 8),
                Piece(PieceNames.Of().Rook, Colors.Of().White, 1, 1)));
            board = await Move(board.Id, 2, 1, 3, 3);
            board = await Move(board.Id, 2, 8, 3, 6);
            board = await Move(board.Id, 3, 3, 2, 1);
            board = await Move(board.Id, 3, 6, 2, 8);
            board = await Move(board.Id, 2, 1, 3, 3);
            board = await Move(board.Id, 2, 8, 3, 6);
            board = await Move(board.Id, 3, 3, 2, 1);
            board = await Move(board.Id, 3, 6, 2, 8);
            Assert.AreEqual("DrawThreefoldRepetition", board.GameStatus);
        }

        [TestMethod]
        public async Task OnlyKingsAndOneMinorPieceEndsInInsufficientMaterialDraw()
        {
            var board = await CreateBoard(Position(
                Piece(PieceNames.Of().King, Colors.Of().White, 5, 1),
                Piece(PieceNames.Of().King, Colors.Of().Black, 5, 8),
                Piece(PieceNames.Of().Bishop, Colors.Of().White, 3, 1)));
            board = await Move(board.Id, 3, 1, 4, 2);
            Assert.AreEqual("DrawInsufficientMaterial", board.GameStatus);
        }

        [TestMethod]
        public void FiftyMoveRuleStartsAtOneHundredHalfMoves()
        {
            Assert.IsFalse(Chess.Domain.DomianModel.ChessModel.Rules.ChessRules.IsFiftyMoveDraw(99));
            Assert.IsTrue(Chess.Domain.DomianModel.ChessModel.Rules.ChessRules.IsFiftyMoveDraw(100));
        }

        [TestMethod]
        public async Task HalfMoveClockCountsQuietMovesAndResetsAfterPawnMove()
        {
            var board = await CreateBoard(Position(
                Piece(PieceNames.Of().King, Colors.Of().White, 5, 1),
                Piece(PieceNames.Of().King, Colors.Of().Black, 5, 8),
                Piece(PieceNames.Of().Rook, Colors.Of().White, 1, 1),
                Piece(PieceNames.Of().Rook, Colors.Of().Black, 8, 8),
                Piece(PieceNames.Of().Pawn, Colors.Of().White, 2, 2)));
            board = await Move(board.Id, 1, 1, 1, 2);
            board = await Move(board.Id, 8, 8, 8, 7);
            Assert.AreEqual(2, board.HalfMoveClock);
            board = await Move(board.Id, 2, 2, 2, 3);
            Assert.AreEqual(0, board.HalfMoveClock);
        }

        [TestMethod]
        [ExpectedException(typeof(DomainError))]
        public async Task BlackCannotMoveBeforeWhiteInAStandardGame()
        {
            var board = await CreateBoard(ChessExtensions.BuildBoard().PlaceAllPieces());
            await Move(board.Id, 1, 7, 1, 6);
        }

        private async Task<Board> CreateBoard(IReadOnlyList<Block> blocks)
        {
            var boardId = BoardId.New;
            await _commandBus.PublishAsync(new CreateBoardCommand(boardId, blocks), CancellationToken.None);
            return await _queryProcessor.ProcessAsync(new GetBoardQuery(boardId), CancellationToken.None);
        }

        private async Task<Board> Move(BoardId boardId, uint fromX, uint fromY, uint toX, uint toY, string promotion = null)
        {
            var board = await _queryProcessor.ProcessAsync(new GetBoardQuery(boardId), CancellationToken.None);
            var piece = PieceAt(board, fromX, fromY);
            Assert.IsNotNull(piece, $"Expected a piece at ({fromX}, {fromY}).");
            var result = await _commandBus.PublishAsync(
                new MovePieceCommand(boardId, new Move(piece.Id, toX, toY, promotion)), CancellationToken.None);
            Assert.IsTrue(result.IsSuccess);
            return await _queryProcessor.ProcessAsync(new GetBoardQuery(boardId), CancellationToken.None);
        }

        private static IReadOnlyList<Block> Position(params ChessPiece[] pieces)
        {
            var blocks = ChessExtensions.BuildBoard().ToList();
            foreach (var piece in pieces) blocks.PlacePiece(piece.XCoordinate, piece.YCoordinate, piece);
            return blocks;
        }

        private static ChessPiece Piece(PieceName name, Color color, uint x, uint y)
        {
            return new ChessPiece
            {
                Id = ChessPieceId.New,
                PieceName = name,
                PieceColor = color,
                XCoordinate = x,
                YCoordinate = y
            };
        }

        private static ChessPiece PieceAt(Board board, uint x, uint y) =>
            board.Blocks.First(b => b.XCoordinate == x && b.YCoordinate == y).ChessPiece;
    }
}
