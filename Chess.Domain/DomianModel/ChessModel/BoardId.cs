using Microservice.Framework.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Domain.DomianModel.ChessModel
{
    [JsonConverter(typeof(SingleValueObjectConverter))]
    public class BoardId : Identity<BoardId>
    {
        #region Constructors

        public BoardId(string value)
            : base(value)
        {

        }

        #endregion
    }
}
