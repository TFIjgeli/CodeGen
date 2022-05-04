using CodeGen.Application.Configurations.Interfaces;
using CodeGen.Domain.Common;
using Dapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGen.Application.DatabaseEntity.Queries.GetAllTablesQuery
{
    public class GetAllTablesQueryHandler : IRequestHandler<GetAllTablesQuery, Response<GetAllTablesQueryDto>>
    {
        private readonly SqlConnection _sqlConnection;

        public GetAllTablesQueryHandler(IConnectionConfigurations connectionString)
        {
            this._sqlConnection = new SqlConnection(connectionString.DbConnectionString());
        }

        public async Task<Response<GetAllTablesQueryDto>> Handle(GetAllTablesQuery request, CancellationToken cancellationToken)
        {
            var result = new GetAllTablesQueryDto();

            var sql = "SELECT TABLE_NAME FROM information_schema.tables; ";

            var res = await _sqlConnection.QueryFirstOrDefaultAsync<GetAllTablesQueryDto>(sql);

            return await Task.FromResult(Response.Success(res));
        }
    }
}
