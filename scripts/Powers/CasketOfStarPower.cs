using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using marisamod.Scripts.Cards.Colorless;

namespace marisamod.scripts.Powers
{
    public class CasketOfStarPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Counter;

        public override async Task AfterBlockGained(Creature creature, decimal amount, ValueProp props, CardModel? cardSource)
        {
            if (!(amount <= 0m) && creature == Owner && Owner.Player != null)
            {
                await Spark.CreateInHand(Owner.Player, Amount, CombatState);
            }
        }
    }
}