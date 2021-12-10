using Chess.Domain.DomianModel.ChessModel;
using Chess.Domain.DomianModel.ChessModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chess.Api.Models.RequestModels
{
    public class MoveRequestModel
    {
        public BoardId BoardId { get; set; }

        public ChessPieceId ChessPieceId { get; set; }

        public uint NewXCoordinate { get; set; }

        public uint NewYCoordinate { get; set; }
    }
}
