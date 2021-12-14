using Chess.Domain;
using Chess.Domain.DomianModel.ChessModel;
using Chess.Domain.DomianModel.ChessModel.Commands;
using Chess.Domain.DomianModel.ChessModel.Queries;
using Chess.Domain.DomianModel.ChessModel.ValueObjects;
using Chess.Tests.Extensions;
using Microservice.Framework.Domain.Commands;
using Microservice.Framework.Domain.Exceptions;
using Microservice.Framework.Domain.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Chess.Tests
{
    [TestClass]
    public class PawnTestFixtures
    {
        private IServiceProvider _provider;
        private IServiceCollection _serviceCollection;

        [TestInitialize]
        public void Startup()
        {
            _serviceCollection = new ServiceCollection();
            _serviceCollection
                .AddLogging(l => l.AddConsole())
                .ConfigureChessDomain()
                .ConfigureChessTestPersistence();
        }

        [TestMethod]
        [Ignore]
        public async Task TestMovePawnOneUpSuccess()
        {
            await TestMove(2, 2, 2, 3);
        }

        [TestMethod]
        [Ignore]
        public async Task TestMovePawnTwoUpSuccess()
        {
            await TestMove(2, 2, 2, 4);
        }

        [TestMethod]
        [Ignore]
        [ExpectedException(typeof(DomainError))]
        public async Task TestMovePawnMoreThanTwoFail()
        {
            await TestMove(2, 2, 2, 5);
        }

        [TestMethod]
        [ExpectedException(typeof(DomainError))]
        public async Task TestMovePawnDiagonalUpRightFail()
        {
            await TestMove(2, 2, 3, 3);
        }

        [TestMethod]
        [ExpectedException(typeof(DomainError))]
        public async Task TestMovePawnDiagonalUpLeftFail()
        {
            await TestMove(2, 2, 1, 3);
        }

        [TestMethod]
        [ExpectedException(typeof(DomainError))]
        public async Task TestMovePawnDiagonalDownLeftFail()
        {
            await TestMove(2, 2, 1, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(DomainError))]
        public async Task TestMovePawnDiagonalDownRightFail()
        {
            await TestMove(2, 2, 3, 1);
        }

        #region Private Methods

        private async Task TestMove(int x_origin, int y_origin, int x_des, int y_des)
        {
            using var provider = _serviceCollection.BuildServiceProvider();
            var commandBus = provider.GetRequiredService<ICommandBus>();
            var queryProcessor = provider.GetRequiredService<IQueryProcessor>();

            var boardId = BoardId.New;
            
            await commandBus
                .PublishAsync(new CreateBoardCommand(boardId), CancellationToken.None);
            
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
