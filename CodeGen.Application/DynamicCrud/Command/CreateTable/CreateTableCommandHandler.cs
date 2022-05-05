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

namespace CodeGen.Application.DynamicCrud.Command.CreateTable
{
    public class CreateTableCommandHandler : IRequestHandler<CreateTableCommand, Response<bool>>
    {
        private readonly SqlConnection _connection;

        public CreateTableCommandHandler(IConnectionConfigurations connectionString)
        {
            _connection = new SqlConnection(connectionString.DbConnectionString());
        }

        public async Task<Response<bool>> Handle(CreateTableCommand request, CancellationToken cancellationToken)
        {
            var storedProc = "UpdateField";

            var dict = new Dictionary<string, object>();
            foreach (var item in request.ColumnValues)
            {
                dict.Add(item.Column, item.Value);
            }

            var results = await _connection.ExecuteAsync(storedProc,
                                                         dict, 
                                                         commandType: CommandType.StoredProcedure);

            return await Task.FromResult(Response.Success(true));
        }
    }
}
