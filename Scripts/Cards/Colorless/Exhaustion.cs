using BaseLib.Utils;
using marisamod.Scripts.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace marisamod.Scripts.Cards.Colorless
{

    [Pool(typeof(TokenCardPool))]
    public class Exhaustion : AbstractMarisaCard
    {
        public Exhaustion() : base(-1, CardType.Status, CardRarity.Status, TargetType.None)
        {
        }

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Unplayable];

        public override decimal ModifyPowerAmountGiven(PowerModel power, Creature giver, decimal amount, Creature? target, CardModel? cardSource)
        {
            if (power is ChargeUpPower && target == Owner.Creature && Owner.PlayerCombatState != null && Owner.PlayerCombatState.Hand.Cards.Contains(this))
            {
                return 0;
            }

            return base.ModifyPowerAmountGiven(power, giver, amount, target, cardSource);
        }
    }
}