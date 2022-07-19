using CodeGen.Application.Configurations.Interfaces;
using CodeGen.Application.DynamicCrud.Command.UpdateTable;
using CodeGen.Application.DynamicCrud.Queries.GetTable;
using CodeGen.Domain.Common;
using Dapper;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGen.Application.DatabaseEntity.Queries.GenerateTableSelect
{
    public class GenerateTableQueryHandler : IRequestHandler<GenerateTableSelectQuery, Response<string>>
    {
        private readonly IConnectionConfigurations _connectionString;
        private readonly IMediator _mediator;
        private readonly SqlConnection _connection;

        public GenerateTableQueryHandler(IConnectionConfigurations connectionString, IMediator mediator)
        {
            this._connectionString = connectionString;
            this._mediator = mediator;
            _connection = new SqlConnection(connectionString.DbConnectionString());
        }


        public async Task<Response<string>> Handle(GenerateTableSelectQuery request, CancellationToken cancellationToken)
        {
            // Get primary key
            var primaryQuery = $" select C.COLUMN_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS T JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE C ON C.CONSTRAINT_NAME=T.CONSTRAINT_NAME WHERE " +
                               $" C.TABLE_NAME = '{request.TableName}' and T.CONSTRAINT_TYPE = 'PRIMARY KEY'";
            var primaryKey = await _connection.QueryFirstOrDefaultAsync<string>(primaryQuery);

            // Construct query
            var query = $" SELECT ROW_NUMBER() OVER (ORDER BY {primaryKey}) AS NUMBER, {TableFields(request.TableFields)} \n" +
                        $" FROM {request.TableName} {JoinObject(request.JoinQuery)} \n" +
                        $" where ({request.TableName}.DeletedFlag = 0) {FilterObject(request.FilterQuery)} {SearchObject(request.SearchQuery)} ";

            if (request.PageSize == null)
            {
                request.CurrentPage = 1;
                request.PageSize = 20;
            }

            query = $" SELECT * FROM \n" +
                    $" ( \n" +
                    $" {query} \n" +
                    $" )\n" +
                    $" AS TBL WHERE NUMBER BETWEEN(({request.CurrentPage} - 1) * {request.PageSize} + 1) AND({request.PageSize} * {request.CurrentPage})";

            var result = this.ProcedureCreation(request.SearchQuery, request.FilterQuery, query);
            var r = _connection.Execute(result);
            return await Task.FromResult(Response.Success(r.ToString()));
        }



        private string JoinObject(string joinFilter)
        {
            try
            {
                var results = string.Empty;

                if (string.IsNullOrEmpty(joinFilter) || joinFilter == "[]")
                    return results;

                var filters = JsonConvert.DeserializeObject<List<JoinTableVM>>(joinFilter);

                foreach (var tables in filters)
                {
                    results = $" {results} LEFT JOIN {tables.ForeignTableName} ON {tables.PrimaryTableName}.{tables.PrimaryTableKey} = {tables.ForeignTableName}.{tables.ForeignTableKey} \n";
                }


                return results;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private string TableFields(string tableFields)
        {
            try
            {
                var results = string.Empty;

                if (string.IsNullOrEmpty(tableFields) || tableFields == "[]")
                    return "* ";

                var filters = JsonConvert.DeserializeObject<List<TableFields>>(tableFields);
                var count = 0;
                foreach (var item in filters)
                {
                    results = $"{results} \n {item.TableName}.{item.Field} ";

                    count++;
                    if (count != filters.Count())
                        results = $"{results}, ";
                }

                return $"{results}";
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private string SearchObject(string search)
        {
            try
            {
                var results = string.Empty;

                //if (string.IsNullOrEmpty(search) || search == "[]")
                //    return results;

                var filters = JsonConvert.DeserializeObject<List<ColumnValue>>(search);

                var count = 0;
                foreach (var item in filters)
                {
                    results = $"{results} \n {item.TableName}.{item.Column} LIKE '%@{item.TableName}{item.Column}Search%' ";

                    count++;
                    if (count != filters.Count())
                        results = $"{results} OR ";
                }

                return $"AND ({results})";
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public string FilterObject(string filter)
        {
            try
            {
                var results = string.Empty;

                //if (string.IsNullOrEmpty(filter) || filter == "[]")
                //    return results;

                var filters = JsonConvert.DeserializeObject<List<ColumnValue>>(filter);

                var count = 0;
                foreach (var item in filters)
                {
                    results = $"{results} \n {item.TableName}.{item.Column} = @{item.TableName}{item.Column}Filter";

                    count++;
                    if (count != filters.Count())
                        results = $"{results} OR ";
                }

                return $" AND ({results})";
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }


        public string ProcedureCreation(string searchQuery, string filterQuery, string query)
        {
            var result = $"CREATE PROCEDURE PROCNAME";

            if (!string.IsNullOrEmpty(searchQuery) && searchQuery != "[]")
            {
                var search = JsonConvert.DeserializeObject<List<ColumnValue>>(searchQuery);
                foreach (var item in search)
                {
                    result = $"{result} \n @{item.TableName}{item.Column}Search varchar(max)";
                }
            }

            if (!string.IsNullOrEmpty(filterQuery) && filterQuery != "[]")
            {
                var filters = JsonConvert.DeserializeObject<List<ColumnValue>>(filterQuery);
                foreach (var item in filters)
                {
                    result = $"{result} \n @{item.TableName}{item.Column}Filter varchar(max)";
                }
            }

            return $"{result} \n" +
                   $"AS \n" +
                   $"BEGIN \n" +
                   $"\n" +
                   $"{query} \n" +
                   $"\n" +
                   $"END";
        }
    }
}
