using Microservice.Framework.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Domain.DomianModel.ChessModel.Entities
{
    [JsonConverter(typeof(SingleValueObjectConverter))]
    public class BlockId : Identity<BlockId>
    {
        #region Constructors

        public BlockId(string value)
            : base(value)
        {

        }

        #endregion
    }
}
