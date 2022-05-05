using CodeGen.Application.DynamicCrud.Command.UpdateTable;
using CodeGen.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGen.Application.DynamicCrud.Command.CreateTable
{
    public class CreateTableCommand : IRequest<Response<bool>>
    {
        public string TableName { get; set; }
        public List<ColumnValue> ColumnValues { get; set; }
    }
}
