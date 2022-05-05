using CodeGen.Application.Configurations.Interfaces;
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

namespace CodeGen.Application.DatabaseEntity.Queries.GetTablesInfo
{
    public class GetTablesInfoQueryHandler : IRequestHandler<GetTablesInfoQuery, Response<List<GetTablesInfoQueryDto>>>
    {
        private readonly IConnectionConfigurations connectionString;
        private SqlConnection _connection;

        public GetTablesInfoQueryHandler(IConnectionConfigurations connectionString)
        {
            this.connectionString = connectionString;
            _connection = new SqlConnection(connectionString.DbConnectionString());
        }

        public async Task<Response<List<GetTablesInfoQueryDto>>> Handle(GetTablesInfoQuery request, CancellationToken cancellationToken)
        {
            var storedProc = "GetFieldProperties";

            var results = await _connection.QueryAsync<GetTablesInfoQueryDto>(storedProc,
                                        new
                                        {
                                            TableName = request.TableName
                                        }, commandType: CommandType.StoredProcedure);

            return await Task.FromResult(Response.Success(results.ToList()));
        }
    }
}
