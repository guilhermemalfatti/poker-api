using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortisService.Core.Models.Messages
{
    public class ErrorResponseMessage : ResponseMessage
    {
        public IList<string> Errors { get; set; } = new List<string>();
    }
}
