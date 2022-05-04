using CodeGen.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGen.Application.DynamicCrud.Queries
{
    public class GetTableQuery : IRequest<Response<List<object>>>
    {
        public GetTableQuery(string tableName)
        {
            TableName = tableName;
        }

        public string TableName { get; set; }

    }
}
