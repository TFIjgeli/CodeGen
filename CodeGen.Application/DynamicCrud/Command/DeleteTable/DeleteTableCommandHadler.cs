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

namespace CodeGen.Application.DynamicCrud.Command.DeleteTable
{
    internal class DeleteTableCommandHadler : IRequestHandler<DeleteTableCommand, Response<bool>>
    {
        private readonly IConnectionConfigurations connectionString;
        private readonly SqlConnection _connection;

        public DeleteTableCommandHadler(IConnectionConfigurations connectionString)
        {
            this.connectionString = connectionString;
            _connection = new SqlConnection(connectionString.DbConnectionString());
        }

        public async Task<Response<bool>> Handle(DeleteTableCommand request, CancellationToken cancellationToken)
        {
            var primaryQuery = $" select C.COLUMN_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS T JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE C ON C.CONSTRAINT_NAME=T.CONSTRAINT_NAME WHERE " +
                               $" C.TABLE_NAME = '{request.TableName}' and T.CONSTRAINT_TYPE = 'PRIMARY KEY'";
            var primaryKey = await _connection.QueryFirstOrDefaultAsync<string>(primaryQuery);

            var query = $"UPDATE {request.TableName} SET DeletedFlag = 1 WHERE {primaryKey} = {request.Id}";

            var results = await _connection.ExecuteAsync(query,
                                                           new
                                                           {
                                                               Id = request.Id,
                                                               TableName = request.TableName,
                                                           });

            return await Task.FromResult(Response.Success(true));
        }
    }
}
