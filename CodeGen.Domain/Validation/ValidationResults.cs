using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGen.Domain.Validation
{
    public class ValidationResults
    {
        public bool IsSuccessful { get; set; } = true;
        public string Error { get; init; }

        public static ValidationResults Success => new ValidationResults();
        public static ValidationResults Fail(string error) => new ValidationResults { IsSuccessful = false, Error = error };
    }
}
