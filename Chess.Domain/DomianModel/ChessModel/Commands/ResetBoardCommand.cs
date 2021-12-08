using Chess.Domain.DomianModel.ChessModel.Entities;
using Chess.Domain.DomianModel.ChessModel.ValueObjects;
using Microservice.Framework.Domain.Commands;
using Microservice.Framework.Domain.ExecutionResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chess.Domain.DomianModel.ChessModel.Commands
{
    public class ResetBoardCommand
        : Command<Board, BoardId>
    {
        #region Constructors

        public ResetBoardCommand(BoardId boardId)
            : base(boardId)
        {
        }

        #endregion
    }

    public class ResetBoardCommandHandler
        : CommandHandler<Board, BoardId, MovePieceCommand>
    {
        #region Virtual Methods

        public override Task<IExecutionResult> ExecuteAsync(
            Board aggregate,
            MovePieceCommand command, 
            CancellationToken cancellationToken)
        {
            aggregate.ResetBoard();
            return Task.FromResult(ExecutionResult.Success());
        }

        #endregion
    }
}
