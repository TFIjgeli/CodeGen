using CodeGen.Application.Configurations.Interfaces;
using CodeGen.Application.DatabaseEntity.Queries.GetTablesInfo;
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

namespace CodeGen.Application.DynamicCrud.Queries
{
    public class GetTableQueryHandler : IRequestHandler<GetTableQuery, Response<List<object>>>
    {
        private readonly IConnectionConfigurations _connectionString;
        private readonly IMediator _mediator;
        private readonly SqlConnection _connection;

        public GetTableQueryHandler(IConnectionConfigurations connectionString, IMediator mediator)
        {
            this._connectionString = connectionString;
            this._mediator = mediator;
            _connection = new SqlConnection(connectionString.DbConnectionString());
        }

        public async Task<Response<List<object>>> Handle(GetTableQuery request, CancellationToken cancellationToken)
        {
            var res = await _mediator.Send(new GetTablesInfoQuery("", request.TableName));
            var s = res.Data;

            var storedProc = $"GetDynamicTable";

            var results = await _connection.QueryAsync<object>(storedProc,
                                                               new
                                                               {
                                                                   TableName = request.TableName,
                                                               }, commandType: CommandType.StoredProcedure);

            return await Task.FromResult(Response.Success(results.ToList()));
        }
    }
}
