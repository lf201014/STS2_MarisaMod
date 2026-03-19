using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using marisamod.Scripts.Cards.Colorless;
using marisamod.Scripts.Powers;

namespace marisamod.Scripts.Powers
{
    public class CasketOfStarPlusPower : AbstractMarisaPower
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Counter;

        public override async Task AfterBlockGained(Creature creature, decimal amount, ValueProp props, CardModel? cardSource)
        {
            if (!(amount <= 0m) && creature == Owner && Owner.Player != null)
            {
                var sparks = await Spark.CreateInHand(Owner.Player, Amount, CombatState);
                foreach (var spark in sparks)
                {
                    CardCmd.Upgrade(spark);
                }
            }
        }
    }
}