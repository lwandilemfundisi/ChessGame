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
    public class BoardTestFixtures
    {
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
        public async Task TestCreatingBoard()
        {
            using var provider = _serviceCollection.BuildServiceProvider();

            var boardId = BoardId.New;
            var commandBus = provider.GetRequiredService<ICommandBus>();
            var commandResult = await commandBus
                .PublishAsync(new CreateBoardCommand(boardId), CancellationToken.None);
            Assert.IsTrue(commandResult.IsSuccess);

            var queryProcessor = provider.GetRequiredService<IQueryProcessor>();
            var queryResult = await queryProcessor
                .ProcessAsync(new GetBoardQuery(boardId), CancellationToken.None);
            Assert.IsNotNull(queryResult);
        }
    }
}
