using CodeGen.Application.Configurations.Interfaces;
using CodeGen.Application.DatabaseEntity.Queries.GetTablesInfo;
using CodeGen.Application.DynamicCrud.Command.CreateTable;
using CodeGen.Application.DynamicCrud.Command.UpdateTable;
using CodeGen.Application.DynamicCrud.Queries.GetTable;
using CodeGen.Domain.Common;
using Dapper;
using MediatR;
using Newtonsoft.Json;
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
            // Get primary key
            var primaryQuery = $" select C.COLUMN_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS T JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE C ON C.CONSTRAINT_NAME=T.CONSTRAINT_NAME WHERE " +
                               $" C.TABLE_NAME = '{request.TableName}' and T.CONSTRAINT_TYPE = 'PRIMARY KEY'";
            var primaryKey = await _connection.QueryFirstOrDefaultAsync<string>(primaryQuery);

            // Construct query
            var query = $"SELECT ROW_NUMBER() OVER (ORDER BY {primaryKey}) AS NUMBER, {TableFields(request.TableFields)} FROM {request.TableName} {JoinObject(request.JoinQuery)}" +
                        $" where ({request.TableName}.DeletedFlag = 0) {FilterObject(request.FilterQuery)} {SearchObject(request.SearchQuery)} ";

            if (request.PageSize == null)
            {
                request.CurrentPage = 1;
                request.PageSize = 20;
            }

            query = $"SELECT * FROM ( {query} ) AS TBL WHERE NUMBER BETWEEN(({request.CurrentPage} - 1) * {request.PageSize} + 1) AND({request.PageSize} * {request.CurrentPage})";

            // Connect to database
            var list = await _connection.QueryAsync<object>(query);
            var records = await _connection.QueryFirstOrDefaultAsync<int>($"SELECT COUNT(*) from {request.TableName}");

            var result = new Pagination<object>(list, records, request.CurrentPage.Value, request.PageSize.Value);
            return await Task.FromResult(Response.Success(result));
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
                    results = $" {results} LEFT JOIN {tables.ForeignTableName} ON {tables.PrimaryTableName}.{tables.PrimaryTableKey} = {tables.ForeignTableName}.{tables.ForeignTableKey} ";
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
                    results = $"{results} {item.TableName}.{item.Field} ";

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

                if (string.IsNullOrEmpty(search) || search == "[]")
                    return results;

                var filters = JsonConvert.DeserializeObject<List<ColumnValue>>(search);

                var count = 0;
                foreach (var item in filters)
                {
                    results = $"{results} {item.TableName}.{item.Column} LIKE '%{item.Value}%'";
                    
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

                if (string.IsNullOrEmpty(filter) || filter == "[]")
                    return results;

                var filters = JsonConvert.DeserializeObject<List<ColumnValue>>(filter);
                
                var count = 0;
                foreach (var item in filters)
                {
                    results = $"{results} {item.TableName}.{item.Column} = '{item.Value}'";

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
