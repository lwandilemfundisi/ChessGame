using Microservice.Framework.Domain.Queries;
using Microservice.Framework.Persistence;
using Microservice.Framework.Persistence.EFCore.Queries.CriteriaQueries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chess.Domain.DomianModel.ChessModel.Queries
{
    public class GetBoardQuery
        : EFCoreCriteriaDomainQuery<Board>, IQuery<Board>
    {
        public GetBoardQuery(
            BoardId boardId)
        {
            Id = boardId;
        }
    }

    public class GetBoardQueryHandler
        : EFCoreCriteriaDomainQueryHandler<Board>, IQueryHandler<GetBoardQuery, Board>
    {
        public GetBoardQueryHandler(IPersistenceFactory persistenceFactory)
            : base(persistenceFactory)
        {
        }

        public Task<Board> ExecuteQueryAsync(
            GetBoardQuery query,
            CancellationToken cancellationToken)
        {
            return Find(query);
        }
    }
}
