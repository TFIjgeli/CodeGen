using CodeGen.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGen.Application.DynamicCrud.Queries
{
    public class GetTableQuery : IRequest<Response<Pagination<object>>>
    {
        public GetTableQuery(string tableName, int? currentPage = null, int? pageSize = null, string filter = "", string joinFilter = "")
        {
            TableName = tableName;
            CurrentPage = currentPage;
            PageSize = pageSize;
            Filter = filter;
            JoinFilter = joinFilter;
        }

        public string TableName { get; set; }
        public int? CurrentPage { get; set; }
        public int? PageSize { get; set; }
        public string Filter { get; }
        public string JoinFilter { get; }
    }
}
