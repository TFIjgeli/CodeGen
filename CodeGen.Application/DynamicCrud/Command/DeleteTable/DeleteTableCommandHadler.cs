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
            var query = $"UPDATE {request.TableName} SET Deleted = 1 WHERE id = {request.Id}";

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
