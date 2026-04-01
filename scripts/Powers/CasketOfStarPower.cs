using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using marisamod.Scripts.Cards.Colorless;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace marisamod.Scripts.Powers
{
    public class CasketOfStarPower : AbstractMarisaPower
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Counter;

        protected const int Threshold = 3;

        public override async Task AfterBlockGained(Creature creature, decimal amount, ValueProp props,
            CardModel? cardSource)
        {
            if (amount >= Threshold && creature == Owner && Owner.Player != null)
            {
                await Spark.CreateInHand(Owner.Player, Amount, CombatState);
            }
        }

        public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            foreach (var allCard in Owner.Player!.PlayerCombatState!.AllCards.Where(x => x is Spark))
            {
                await ApplyRetain(allCard);
            }
        }

        public override async Task AfterCardEnteredCombat(CardModel card)
        {
            if (card.Owner == Owner.Player && card is Spark)
            {
                await ApplyRetain(card);
            }
        }

        private static Task ApplyRetain(CardModel card)
        {
            if (!card.Keywords.Contains(CardKeyword.Retain))
            {
                CardCmd.ApplyKeyword(card, CardKeyword.Retain);
            }

            return Task.CompletedTask;
        }
    }
}