using FortisService.Core.Abstractions;
using FortisService.Models.Enumerator;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FortisService.Models.Models.Tables
{
    public class Card : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        public Rank Rank { get; set; }

        [Required]
        public Suit Suit { get; set; }

        [JsonIgnore]
        public IList<StatusHistory> FirstCards { get; set; }

        [JsonIgnore]
        public IList<StatusHistory> SecondCards { get; set; }

        [JsonIgnore]
        public IList<StatusHistory> ThirdCards { get; set; }

        [JsonIgnore]
        public IList<StatusHistory> FourthCards { get; set; }

        [JsonIgnore]
        public IList<StatusHistory> FifthCards { get; set; }

        public override string ToString()
        {
            return $"{Rank} of {Suit}";
        }
    }
}
