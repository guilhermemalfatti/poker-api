using FluentValidation;
using FortisService.Models.Payloads;

namespace FortisPokerCard.WebService.Validators
{
    public class GameEntryValidator : AbstractValidator<GameEntry>
    {
        public GameEntryValidator()
        {
            RuleFor(entry => entry.Key).NotEmpty();
            RuleFor(entry => entry.PlayerIds).NotEmpty();
            RuleFor(entry => entry.PlayerIds.Count).GreaterThan(1);
        }
    }
}
