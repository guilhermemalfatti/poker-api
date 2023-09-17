using FortisService.Core.Abstractions;

namespace FortisService.Core.Models.Tables
{
    public class Game : ITrackable, IEntity
    {
        public int Id { get; set; }

        public string? Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}
