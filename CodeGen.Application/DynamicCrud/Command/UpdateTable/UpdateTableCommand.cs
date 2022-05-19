using CodeGen.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGen.Application.DynamicCrud.Command.UpdateTable
{
    public class UpdateTableCommand : IRequest<Response<bool>>
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public List<ColumnValue> ColumnValues { get; set; }
    }

    public class ColumnValue
    {
        public string TableName { get; set; }
        public string Column { get; set; }
        public string Value { get; set; }
    }
}
