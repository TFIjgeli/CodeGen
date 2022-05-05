using CodeGen.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGen.Application.DynamicCrud.Queries.GetTableById
{
    public record GetTableByIdQuery(int Id, string TableName) : IRequest<Response<object>>;
}
