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
    public class MovePieceCommand
        : Command<Board, BoardId>
    {
        #region Constructors

        public MovePieceCommand(
            BoardId boardId,
            Move move)
            : base(boardId)
        {
            Move = move;
        }

        #endregion

        #region Properties

        public Move Move { get; }

        #endregion
    }

    public class MovePieceCommandHandler
        : CommandHandler<Board, BoardId, MovePieceCommand>
    {
        #region Virtual Methods

        public override Task<IExecutionResult> ExecuteAsync(
            Board aggregate,
            MovePieceCommand command, 
            CancellationToken cancellationToken)
        {
            aggregate.MovePiece(command.Move);
            return Task.FromResult(ExecutionResult.Success());
        }

        #endregion
    }
}
