using FortisService.Core.Abstractions;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using FortisService.Models.Enumerator;
using System.Collections;
using System.Collections.Generic;
using FortisService.Core.Models.Tables;
using System.Text.Json.Serialization;

namespace FortisService.Models.Models.Tables
{
    public class StatusHistory : ITrackable, IEntity
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Status Status { get; set; }

        public bool Winner { get; set; }

        public int? FirstCardId { get; set; }

        public int? SecondCardId { get; set; }

        public int? ThirdCardId { get; set; }

        public int? FourthCardId { get; set; }

        public int? FifthCardId { get; set; }

        public int PlayerId { get; set; }

        public int GameId { get; set; }

        [JsonIgnore]
        public Player Player { get; set; }

        [JsonIgnore]
        public Game Game { get; set; }

        [JsonIgnore]
        public Card FirstCard { get; set; }

        [JsonIgnore]
        public Card SecondCard { get; set; }

        [JsonIgnore]
        public Card ThirdCard { get; set; }

        [JsonIgnore]
        public Card FourthCard { get; set; }

        [JsonIgnore]
        public Card FifthCard { get; set; }

        /*public IList<Card> Hand { get; set; } // todo check how it will be mapped in the DB*/

        public DateTime CreatedAt { get; set; }

        public DateTime LastUpdatedAt { get; set; }
    }
}
