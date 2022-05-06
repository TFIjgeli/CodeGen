using CodeGen.Application.DynamicCrud.Command.UpdateTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGen.Application.DynamicCrud.Queries.GetTable
{
    public class JoinTableVM
    {
        public string TableName { get; set; }
        public string ForeignTableKey { get; set; }
        public string PrimaryTableKey { get; set; }
        public List<ColumnValue> ColumnValues { get; set; }
    }
}
