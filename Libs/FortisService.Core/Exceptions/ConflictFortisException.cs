using FortisService.Core.Models.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortisService.Core.Exceptions
{
    public class ConflictFortisException : FortisException
    {
        public ConflictFortisException()
        {
        }

        public ConflictFortisException(string message)
            : base(message)
        {
        }

    }
}
