using Chess.Domain.DomianModel.ChessModel;
using Chess.Domain.DomianModel.ChessModel.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Chess.Api.Models.RequestModels
{
    public class MoveRequestModel
    {
        [Required]
        public BoardId BoardId { get; set; }
        [Required]
        public ChessPieceId ChessPieceId { get; set; }
        [Required]
        public uint NewXCoordinate { get; set; }
        [Required]
        public uint NewYCoordinate { get; set; }
    }
}
