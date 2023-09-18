using FortisService.Core.Models.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortisService.Core.Exceptions
{
    public class FortisException : Exception
    {
        public FortisException()
        {
        }
        public FortisException(string message)
            : base(message)
        {
        }

        public FortisException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

    }
}
