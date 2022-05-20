using CodeGen.Application.DynamicCrud.Command.UpdateTable;
using CodeGen.Application.DynamicCrud.Queries.GetTable;
using CodeGen.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGen.Application.DatabaseEntity.Queries.GenerateTableSelect
{
    public class GenerateTableSelectQuery : IRequest<Response<string>>
    {
        public GenerateTableSelectQuery(string tableName,
                                       int? currentPage = null,
                                       int? pageSize = null,
                                       string searchQuery = "",
                                       string joinQuery = "",
                                       string filterQuery = "",
                                       string tableFields = "")
        {
            TableName = tableName;
            CurrentPage = currentPage;
            PageSize = pageSize;
            SearchQuery = searchQuery;
            JoinQuery = joinQuery;
            FilterQuery = filterQuery;
            TableFields = tableFields;
        }

        public string TableName { get; set; }
        public string TableFields { get; set; }

        public int? CurrentPage { get; set; }
        public int? PageSize { get; set; }
        public string SearchQuery { get; set; }
        public string FilterQuery { get; set; }
        public string JoinQuery { get; set; }
    }


    public class TableSelectPost
    {
        //public string TableName { get; set; }
        public List<TableFields> TableFields { get; set; }
        public List<ColumnValue> SearchQuery { get; set; }
        public List<ColumnValue> FilterQuery { get; set; }
        public List<JoinTableVM> JoinQuery { get; set; }


        public int? CurrentPage { get; set; }
        public int? PageSize { get; set; }
    }
}
