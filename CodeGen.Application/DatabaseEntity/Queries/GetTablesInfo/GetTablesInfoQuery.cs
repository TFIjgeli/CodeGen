using CodeGen.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGen.Application.DatabaseEntity.Queries.GetTablesInfo
{
    public class GetTablesInfoQuery : IRequest<Response<List<GetTablesInfoQueryDto>>>
    {
        public string ConnectionString { get; set; }
        public string TableName { get; set; }

        public GetTablesInfoQuery(string connectionString, string tableName)
        {
            ConnectionString = connectionString;
            TableName = tableName;
        }

    }
}
