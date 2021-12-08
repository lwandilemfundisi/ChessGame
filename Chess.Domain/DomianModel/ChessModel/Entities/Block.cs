using Chess.Domain.DomianModel.ChessModel.ValueObjects;
using Chess.Domain.DomianModel.ChessModel.ValueObjects.LookupValueObjects;
using Microservice.Framework.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Domain.DomianModel.ChessModel.Entities
{
    public class Block : Entity<BlockId>
    {
        #region Properties

        public ChessPiece ChessPiece { get; set; }

        public Color BlockColor { get; set; }

        public uint XCoordinate { get; set; }

        public uint YCoordinate { get; set; }

        #endregion
    }
}
