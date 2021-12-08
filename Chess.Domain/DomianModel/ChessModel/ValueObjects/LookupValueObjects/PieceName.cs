using Microservice.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Domain.DomianModel.ChessModel.ValueObjects.LookupValueObjects
{
    [ValueObjectResourcePath("Chess.Domain.DomianModel.ChessModel.ValueObjects.LookupValueObjects.Mappings.PieceName.xml")]
    public class PieceName : XmlValueObject
    {
        #region Properties

        public string Image { get; set; }

        #endregion

        #region Virtual Methods

        protected override XmlValueObject OnCreateClone()
        {
            return new PieceName();
        }

        protected override XmlValueObject OnClone(XmlValueObject valueObject)
        {
            var clone = (PieceName)valueObject;
            clone.Image = Image;
            return base.OnClone(valueObject);
        }

        #endregion
    }

    public class PieceNames : XmlValueObjectLookup<PieceName, PieceNames>
    {
        public PieceName Pawn { get { return FindValueObject("Pawn"); } }
        public PieceName King { get { return FindValueObject("King"); } }
        public PieceName Queen { get { return FindValueObject("Queen"); } }
        public PieceName Bishop { get { return FindValueObject("Bishop"); } }
        public PieceName Night { get { return FindValueObject("Night"); } }
        public PieceName Rook { get { return FindValueObject("Rook"); } }
    }
}
