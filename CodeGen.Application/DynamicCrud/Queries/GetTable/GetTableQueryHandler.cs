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
            var primaryQuery = $" select C.COLUMN_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS T JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE C ON C.CONSTRAINT_NAME=T.CONSTRAINT_NAME WHERE " +
                               $" C.TABLE_NAME = '{request.TableName}' and T.CONSTRAINT_TYPE = 'PRIMARY KEY'";
            var primaryKey = await _connection.QueryFirstOrDefaultAsync<string>(primaryQuery);

            var joins = this.JoinObject(request.JoinFilter, request.TableName);
            var query = $"SELECT ROW_NUMBER() OVER (ORDER BY {primaryKey}) AS NUMBER, * FROM {request.TableName} {joins.JoinTable}" +
                        $" where ({request.TableName}.DeletedFlag = 0) {FilterObject(request.Filter)} {joins.WhereQuery}";

            if (request.PageSize == null)
            {
                request.CurrentPage = 1;
                request.PageSize = 20;
            }
            
            query = $"SELECT * FROM ( {query} ) AS TBL WHERE NUMBER BETWEEN(({request.CurrentPage} - 1) * {request.PageSize} + 1) AND({request.PageSize} * {request.CurrentPage})";


            var list = await _connection.QueryAsync<object>(query);
            var records = await _connection.QueryFirstOrDefaultAsync<int>($"SELECT COUNT(*) from {request.TableName}");
            
            var result = new Pagination<object>(list, records, request.CurrentPage.Value, request.PageSize.Value);
            //result.List = list.ToList();


            //var result = this.GetPagination(list, request.CurrentPage, request.PageSize);

            return await Task.FromResult(Response.Success(result));
        }





        private JoinQueryStringVM JoinObject(string joinFilter, string primaryTable)
        {
            try
            {
                var results = new JoinQueryStringVM();
                var query = string.Empty;
                var join = string.Empty;

                if (joinFilter == null)
                    return new JoinQueryStringVM()
                    {
                        JoinTable = string.Empty,
                        WhereQuery = string.Empty
                    };

                var filters = JsonConvert.DeserializeObject<List<JoinTableVM>>(joinFilter);
                foreach (var tables in filters)
                {
                    join = $" {join} LEFT JOIN {tables.TableName} ON {primaryTable}.{tables.PrimaryTableKey} = {tables.TableName}.{tables.ForeignTableKey} ";

                    foreach (var item in tables.ColumnValues)
                    {
                        query = $"{query} AND {tables.TableName}.{item.Column} = '{item.Value}'";
                    }

                    results.JoinTable = $"{results.JoinTable} {join} ";
                    results.WhereQuery = $"{results.WhereQuery} {query} AND ({tables.TableName}.DeletedFlag = 0)";
                }

                return results;
            }
            catch (Exception)
            {
                return new JoinQueryStringVM()
                {
                    JoinTable = string.Empty,
                    WhereQuery = string.Empty
                };
            }
        }

        private string FilterObject(string filter)
        {
            try
            {
                var query = string.Empty;

                if (filter == null)
                    return string.Empty;

                var filters = JsonConvert.DeserializeObject<List<ColumnValue>>(filter);
                foreach (var item in filters)
                {
                    query = $"{query} AND {item.Column} = '{item.Value}'";
                }

                return query;
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
