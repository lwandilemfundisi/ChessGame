using Chess.Api.Mappers.Concept;
using Chess.Api.Models.RequestModels;
using Chess.Domain.DomianModel.ChessModel.ValueObjects;
using Microservice.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chess.Api.Mappers
{
    public class MoveMapper : Mapper<Move>
    {
        private readonly MoveRequestModel _model;

        #region Constructors

        public MoveMapper(MoveRequestModel model)
        {
            _model = model;
        }

        #endregion

        public override Move Map()
        {
            if (_model.IsNull())
                throw new ArgumentException($"{GetType().PrettyPrint()} : Cannot map null model");

            return new Move(_model.ChessPieceId, _model.NewXCoordinate, _model.NewYCoordinate) { };
        }
    }
}
