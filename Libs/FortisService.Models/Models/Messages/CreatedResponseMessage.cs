using FortisService.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortisService.Core.Models.Messages
{
    public class CreatedResponseMessage<T> : ObjectResponseMessage<T>
    {
        public CreatedResponseMessage()
        {
        }

        public CreatedResponseMessage(T entity)
        {
            Entity = entity;
            Title = $"An entry for {typeof(T).Name} has been created.";
        }
    }
}
