using marisamod.Scripts.Cards.Colorless;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Powers
{
    public class CasketOfStarPlusPower : AbstractMarisaPower
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Counter;
        
        public override async Task AfterBlockGained(Creature creature, decimal amount, ValueProp props,
            CardModel? cardSource)
        {
            if (amount >= CasketOfStarPower.Threshold && creature == Owner && Owner.Player != null)
            {
                var sparks = await Spark.CreateInHand(Owner.Player, Amount, CombatState);
                foreach (var spark in sparks)
                {
                    CardCmd.Upgrade(spark);
                }
            }
        }
        
        public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            foreach (var allCard in Owner.Player!.PlayerCombatState!.AllCards.Where(x => x is Spark))
            {
                await CasketOfStarPower.ApplyRetain(allCard);
            }
        }

        public override async Task AfterCardEnteredCombat(CardModel card)
        {
            if (card.Owner == Owner.Player && card is Spark)
            {
                await CasketOfStarPower.ApplyRetain(card);
            }
        }
    }
}