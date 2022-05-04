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
            var storedProc = "UpdateField";

            foreach (var item in request.ColumnValues)
            {
                var results = await _connection.ExecuteAsync(storedProc,
                                                           new
                                                           {
                                                               Id = request.Id,
                                                               TableName = request.TableName,
                                                               Field = item.Column,
                                                               Value = item.Value,
                                                           }, commandType: CommandType.StoredProcedure);
            }

            return await Task.FromResult(Response.Success(true));
        }
    }
}
