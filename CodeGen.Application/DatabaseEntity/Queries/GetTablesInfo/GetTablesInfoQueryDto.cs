using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGen.Application.DatabaseEntity.Queries.GetTablesInfo
{
    public class GetTablesInfoQueryDto
    {
        public string Column_Name { get; set; }
        public string Data_Type { get; set; }
    }
}
