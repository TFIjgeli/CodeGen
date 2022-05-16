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
        public GetTableQuery(string tableName, int? currentPage = null, int? pageSize = null, string searchQuery = "", string joinQuery = "", string filterQuery = "")
        {
            TableName = tableName;
            CurrentPage = currentPage;
            PageSize = pageSize;
            SearchQuery = searchQuery;
            JoinQuery = joinQuery;
            FilterQuery = filterQuery;
        }

        public string TableName { get; set; }
        public int? CurrentPage { get; set; }
        public int? PageSize { get; set; }
        public string SearchQuery { get; }
        public string FilterQuery { get; set; }
        public string JoinQuery { get; }
    }
}
