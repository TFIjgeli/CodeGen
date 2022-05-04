using CodeGen.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGen.Application.DynamicCrud.Command.DeleteTable
{
    public class DeleteTableCommand : IRequest<Response<bool>>
    {
        public DeleteTableCommand(int id, string tableName)
        {
            Id = id;
            TableName = tableName;
        }

        public int Id { get; set; }
        public string TableName { get; set; }
    }
}
