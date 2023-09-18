using FluentValidation;
using FortisService.Core.Payload.V1;
using FortisService.Models.Payloads;

namespace FortisPokerCard.WebService.Validators
{
    public class PlayerEntryValidator : AbstractValidator<RouteIdParameters>
    {
        public PlayerEntryValidator()
        {
            RuleFor(entry => entry.Id).GreaterThan(0);
        }
    }
}
