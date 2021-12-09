using Chess.Domain.DomianModel.ChessModel.ValueObjects;
using Chess.Domain.DomianModel.ChessModel.ValueObjects.LookupValueObjects;
using Chess.Domain.Extensions;
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
        private ChessPiece _chessPiece;

        #region Properties

        private Action<object, string> LazyLoader { get; set; }

        public ChessPiece ChessPiece
        {
            get => LazyLoader.Load(this, ref _chessPiece);
            set => _chessPiece = value;
        }

        public Color BlockColor { get; set; }

        public uint XCoordinate { get; set; }

        public uint YCoordinate { get; set; }

        #endregion
    }
}
