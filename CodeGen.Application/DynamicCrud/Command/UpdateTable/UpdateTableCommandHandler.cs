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

namespace CodeGen.Application.DynamicCrud.Command.UpdateTable
{
    public class UpdateTableCommandHandler : IRequestHandler<UpdateTableCommand, Response<bool>>
    {
        private readonly IConnectionConfigurations _connectionString;
        private readonly SqlConnection _connection;

        public UpdateTableCommandHandler(IConnectionConfigurations connectionString)
        {
            this._connectionString = connectionString;
            _connection = new SqlConnection(connectionString.DbConnectionString());
        }

        public async Task<Response<bool>> Handle(UpdateTableCommand request, CancellationToken cancellationToken)
        {
            var primaryQuery = $" select C.COLUMN_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS T JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE C ON C.CONSTRAINT_NAME=T.CONSTRAINT_NAME WHERE " +
                               $" C.TABLE_NAME = '{request.TableName}' and T.CONSTRAINT_TYPE = 'PRIMARY KEY'";
            var primaryKey = await _connection.QueryFirstOrDefaultAsync<string>(primaryQuery);

            var query = $"UPDATE {request.TableName} SET {GetValues(request.ColumnValues)} WHERE {primaryKey} = {request.Id}";

            var results = await _connection.ExecuteAsync(query);

            return await Task.FromResult(Response.Success(true));
        }


        private string GetValues(List<ColumnValue> columnValues)
        {
            columnValues = columnValues.Where(c => !string.IsNullOrEmpty(c.Value))
                                       .ToList();

            var res = string.Empty;
            var count = 0;
            foreach (var item in columnValues)
            {
                count++;
                res = $"{res} {item.Column} = '{item.Value}'";

                if (count != columnValues.Count)
                    res = $"{res},";
            }

            return res;
        }
    }
}
