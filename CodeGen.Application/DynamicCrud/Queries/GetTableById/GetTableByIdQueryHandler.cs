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

namespace CodeGen.Application.DynamicCrud.Queries.GetTableById
{
    public class GetTableByIdQueryHandler : IRequestHandler<GetTableByIdQuery, Response<object>>
    {
        private readonly SqlConnection _connection;

        public GetTableByIdQueryHandler(IConnectionConfigurations connectionString)
        {
            _connection = new SqlConnection(connectionString.DbConnectionString());
        }

        public async Task<Response<object>> Handle(GetTableByIdQuery request, CancellationToken cancellationToken)
        {
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
