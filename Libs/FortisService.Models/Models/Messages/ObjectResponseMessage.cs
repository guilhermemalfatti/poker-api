using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortisService.Core.Models.Messages
{
    public abstract class ObjectResponseMessage<T> : ResponseMessage
    {
        public T Entity { get; set; }
    }
}
