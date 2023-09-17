using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortisService.Core.Models.Messages
{
    public class ResponseMessage
    {
        /// <summary>
        /// Http status code number.
        /// </summary>
        public int StatusCode { get; set; } = 200;

        /// <summary>
        /// Message title related to the query.
        /// </summary>
        public string Title { get; set; }

        public TimeSpan QueryElapsedTime { get; set; }
    }
}
