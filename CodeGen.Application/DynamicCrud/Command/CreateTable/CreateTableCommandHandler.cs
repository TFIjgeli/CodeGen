using CodeGen.Application.Configurations.Interfaces;
using CodeGen.Application.DynamicCrud.Command.UpdateTable;
using CodeGen.Domain.Common;
using Dapper;
using MediatR;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;

        public CreateTableCommandHandler(IConnectionConfigurations connectionString, IConfiguration configuration)
        {
            _connection = new SqlConnection(connectionString.DbConnectionString());
            this._configuration = configuration;
        }

        public async Task<Response<bool>> Handle(CreateTableCommand request, CancellationToken cancellationToken)
        {
            request.ColumnValues = request.ColumnValues
                                          .Where(r => !string.IsNullOrEmpty(r.Value))
                                          .ToList();

            var query = $"INSERT INTO {request.TableName} ({this.GetColumn(request.ColumnValues)}, CreateBy) VALUES ({this.GetValues(request.ColumnValues)}, '')";

            //var storedProc = "UpdateField";


            var results = await _connection.ExecuteAsync(query,
                                                         commandType: CommandType.Text);

            return await Task.FromResult(Response.Success(true));
        }

        private string GetColumn(List<ColumnValue> columnValue)
        {
            var str = "";
            var count = 0;
            foreach (var item in columnValue)
            {
                count++;
                str = $"{str} {item.Column}";

                if (count != columnValue.Count)
                    str = $"{str},";
            }

            return str;
        }

        private string GetValues(List<ColumnValue> columnValue)
        {
            var str = "";
            var count = 0;
            foreach (var item in columnValue)
            {
                count++;
                str = $"{str} '{item.Value}'";

                if (count != columnValue.Count)
                    str = $"{str},";
            }

            return str;
        }
    }
}
