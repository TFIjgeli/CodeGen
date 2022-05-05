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
    public class GetTableQueryHandler : IRequestHandler<GetTableQuery, Response<Pagination<object>>>
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

        public async Task<Response<Pagination<object>>> Handle(GetTableQuery request, CancellationToken cancellationToken)
        {
            var storedProc = $"GetDynamicTable";
            var results = await _connection.QueryAsync<object>(storedProc,
                                                               new
                                                               {
                                                                   TableName = request.TableName,
                                                               }, commandType: CommandType.StoredProcedure);

            var result = this.GetPagination(results, request.CurrentPage, request.PageSize);

            return await Task.FromResult(Response.Success(result));
        }





        private Pagination<object> GetPagination(IEnumerable<object> list, int? currentPage, int? pageSize)
        {            
            if (currentPage == null)
            {
                var totalItems = list.Count();
                return new Pagination<object>(list, totalItems, 1, 20);
            }
            else
            {
                var totalItems = list.Count();

                list = list.Skip((currentPage.Value - 1) * pageSize.Value).Take(pageSize.Value);
                return new Pagination<object>(list, totalItems, currentPage.Value, pageSize.Value);
            }
        }
    }
}
