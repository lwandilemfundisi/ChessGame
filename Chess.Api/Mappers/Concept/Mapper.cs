using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chess.Api.Mappers.Concept
{
    public abstract class Mapper<T>
    {
        public abstract T Map();
    }
}
