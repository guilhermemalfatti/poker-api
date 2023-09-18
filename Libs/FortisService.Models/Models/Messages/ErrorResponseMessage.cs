using FortisService.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortisService.Core.Models.Messages
{
    public class ErrorResponseMessage<T> : ObjectResponseMessage<T>
    {
        public ErrorResponseMessage()
        {
            Title = $"Entity of type, {typeof(T).FullName}, already exists.";
        }
    }
}
