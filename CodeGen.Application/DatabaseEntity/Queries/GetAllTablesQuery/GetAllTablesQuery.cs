using CodeGen.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGen.Application.DatabaseEntity.Queries.GetAllTablesQuery
{
    public record GetAllTablesQuery(string connectionString) : IRequest<Response<List<GetAllTablesQueryDto>>>;
}
