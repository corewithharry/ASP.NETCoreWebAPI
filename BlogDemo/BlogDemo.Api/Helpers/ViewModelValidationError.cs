using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogDemo.Api.Helpers
{
    public class ViewModelValidationError
    {
        public string ValidatorKey { get; private set; }
        public string Message { get; private set; }

        public ViewModelValidationError(string message, string validaotrKey = "")
        {
            ValidatorKey = validaotrKey;
            Message = message;
        }
    }
}
