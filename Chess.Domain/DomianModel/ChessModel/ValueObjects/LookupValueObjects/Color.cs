using Microservice.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Domain.DomianModel.ChessModel.ValueObjects.LookupValueObjects
{
    [ValueObjectResourcePath("Chess.Domain.DomianModel.ChessModel.ValueObjects.LookupValueObjects.Mappings.Color.xml")]
    public class Color : XmlValueObject
    {
        #region Properties

        public bool YAxisDirectionIsUp { get; set; }

        #endregion

        #region Virtual Methods

        protected override XmlValueObject OnCreateClone()
        {
            return new Color();
        }

        protected override XmlValueObject OnClone(XmlValueObject valueObject)
        {
            var clone = (Color)valueObject;
            clone.YAxisDirectionIsUp = YAxisDirectionIsUp;
            return base.OnClone(valueObject);
        }

        #endregion
    }

    public class Colors : XmlValueObjectLookup<Color, Colors>
    {
        public Color Black { get { return FindValueObject("Black"); } }

        public Color White { get { return FindValueObject("White"); } }
    }
}
