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
    }

    public class Colors : XmlValueObjectLookup<Color, Colors>
    {
        public Color Black { get { return FindValueObject("Black"); } }

        public Color White { get { return FindValueObject("White"); } }
    }
}
