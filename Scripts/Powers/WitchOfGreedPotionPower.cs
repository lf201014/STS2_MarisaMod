using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;

namespace marisamod.Scripts.Powers
{
    public class WitchOfGreedPotionPower : AbstractMarisaPower
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Counter;

        public override Task AfterCombatEnd(CombatRoom room)
        {
            if (Owner.Player != null)
                for (var i = 0; i < Amount; i++)
                    room.AddExtraReward(Owner.Player, new PotionReward(Owner.Player));
            return Task.CompletedTask;
        }
    }
}