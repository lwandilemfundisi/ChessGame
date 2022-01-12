using Chess.Domain;
using Chess.Domain.DomianModel.ChessModel;
using Chess.Domain.DomianModel.ChessModel.Commands;
using Chess.Domain.DomianModel.ChessModel.Entities;
using Chess.Domain.DomianModel.ChessModel.Queries;
using Chess.Domain.DomianModel.ChessModel.ValueObjects;
using Chess.Domain.Extensions;
using Chess.Persistence.Extensions;
using Chess.Tests.Context;
using Chess.Tests.Extensions;
using Microservice.Framework.Domain.Commands;
using Microservice.Framework.Domain.Exceptions;
using Microservice.Framework.Domain.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Chess.Tests.UnitTests.PawnUnitTests
{
    [TestClass]
    public class PawnTestFixtures
    {
        private IServiceCollection _serviceCollection;

        [TestInitialize]
        public void Startup()
        {
            _serviceCollection = new ServiceCollection();
            _serviceCollection
                .AddLogging(l => l.AddConsole())
                .ConfigureChessDomain()
                .ConfigureChessPersistence<ChessInMemoryContext, TestChessContextProvider>();
        }

        [TestMethod]
        public async Task TestMovePawnOneUpSuccess()
        {
            var blocks = ChessExtensions
                    .BuildBoard()
                    .PlaceWhitePawns();

            await TestMove(blocks, 2, 2, 2, 3);
        }

        [TestMethod]
        public async Task TestMovePawnTwoUpSuccess()
        {
            var blocks = ChessExtensions
                    .BuildBoard()
                    .PlaceWhitePawns();

            await TestMove(blocks, 2, 2, 2, 4);
        }

        [TestMethod]
        [ExpectedException(typeof(DomainError))]
        public async Task TestMovePawnMoreThanTwoFail()
        {
            var blocks = ChessExtensions
                    .BuildBoard()
                    .PlaceWhitePawns();

            await TestMove(blocks, 2, 2, 2, 5);
        }

        [TestMethod]
        [ExpectedException(typeof(DomainError))]
        public async Task TestMovePawnDiagonalUpRightFail()
        {
            var blocks = ChessExtensions
                    .BuildBoard()
                    .PlaceWhitePawns();

            await TestMove(blocks, 2, 2, 3, 3);
        }

        [TestMethod]
        [ExpectedException(typeof(DomainError))]
        public async Task TestMovePawnDiagonalUpLeftFail()
        {
            var blocks = ChessExtensions
                    .BuildBoard()
                    .PlaceWhitePawns();

            await TestMove(blocks, 2, 2, 1, 3);
        }

        [TestMethod]
        [ExpectedException(typeof(DomainError))]
        public async Task TestMovePawnDiagonalDownLeftFail()
        {
            var blocks = ChessExtensions
                .BuildBoard()
                .PlaceWhitePawns();

            await TestMove(blocks, 2, 2, 1, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(DomainError))]
        public async Task TestMovePawnDiagonalDownRightFail()
        {
            var blocks = ChessExtensions
                .BuildBoard()
                .PlaceWhitePawns();

            await TestMove(blocks, 2, 2, 3, 1);
        }

        [TestMethod]
        public async Task TestPawnCaptureOpponentPiece()
        {
            var blocks = ChessExtensions
                .BuildBoard()
                .PlacePawnValidCaptureScenario();

            await TestMove(blocks, 1, 2, 2, 3);
        }

        [TestMethod]
        [ExpectedException(typeof(DomainError))]
        public async Task TestPawnCannotCaptureOwnPiece()
        {
            var blocks = ChessExtensions
                .BuildBoard()
                .PlacePawnInvalidCaptureScenario();

            await TestMove(blocks, 1, 2, 2, 3);
        }

        [TestMethod]
        [ExpectedException(typeof(DomainError))]
        public async Task TestPawnCannotLeapOverPiece()
        {
            var blocks = ChessExtensions
                .BuildBoard()
                .PlaceCannotLeapOverPieceScenario();

            await TestMove(blocks, 1, 2, 1, 4);
        }

        #region Private Methods

        private async Task TestMove(
            IReadOnlyList<Block> blocks,
            int x_origin,
            int y_origin,
            int x_des,
            int y_des)
        {
            using var provider = _serviceCollection.BuildServiceProvider();
            var commandBus = provider.GetRequiredService<ICommandBus>();
            var queryProcessor = provider.GetRequiredService<IQueryProcessor>();

            var boardId = BoardId.New;

            await commandBus
                .PublishAsync(new CreateBoardCommand(boardId, blocks), CancellationToken.None);

            var queryResult = await queryProcessor
                .ProcessAsync(new GetBoardQuery(boardId), CancellationToken.None);

            var pieceId = queryResult
                .Blocks.First(b => b.XCoordinate == x_origin
                && b.YCoordinate == y_origin).ChessPiece.Id;

            var moveResult = await commandBus
                .PublishAsync(
                new MovePieceCommand(boardId, new Move(pieceId, (uint)x_des, (uint)y_des)),
                CancellationToken.None);

            Assert.IsTrue(moveResult.IsSuccess);
        }

        #endregion
    }
}
