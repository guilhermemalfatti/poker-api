using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortisService.Core.Abstractions
{
    public interface ITrackable
    {
        DateTime CreatedAt { get; set; }

        DateTime LastUpdatedAt { get; set; }
    }
}
