using CodeGen.Application.Configurations.Interfaces;
using CodeGen.Application.DynamicCrud.Command.UpdateTable;
using CodeGen.Domain.Common;
using Dapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGen.Application.DynamicCrud.Queries.GetTableById
{
    public class GetTableByIdQueryHandler : IRequestHandler<GetTableByIdQuery, Response<object>>
    {
        private readonly SqlConnection _connection;
        private readonly IConnectionConfigurations _connectionString;

        public GetTableByIdQueryHandler(IConnectionConfigurations connectionString)
        {
            _connection = new SqlConnection(connectionString.DbConnectionString());
            this._connectionString = connectionString;
        }

        public async Task<Response<object>> Handle(GetTableByIdQuery request, CancellationToken cancellationToken)
        {
            //var list = new List<ColumnValue>
            //{
            //    new ColumnValue { Column = "Column 1", Value = "Value 1" },
            //    new ColumnValue { Column = "Column 2", Value = "Value 2" },
            //    new ColumnValue { Column = "Column 3", Value = "Value 3" },
            //};




            //return await Task.FromResult(Response.Success(new object { }));


            var storedProc = $"GetDynamicTableById";
            var results = await _connection.QueryFirstOrDefaultAsync<object>(storedProc,
                                                                               new
                                                                               {
                                                                                   TableName = request.TableName,
                                                                                   Id = request.Id,
                                                                               }, commandType: CommandType.StoredProcedure);

            return await Task.FromResult(Response.Success(results));
        }
    }
}
