using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Powers
{
    public class OrrerysSunPower : AbstractMarisaPower
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Counter;

        private int _rec = 0;


        public override Task BeforeCardPlayed(CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner == Owner.Player)
            {
                _rec = Owner.GetPowerAmount<ChargeUpPower>();
            }

            return Task.CompletedTask;
        }

        public override async Task AfterCardPlayedLate(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner == Owner.Player)
            {
                var cnt = Owner.GetPowerAmount<ChargeUpPower>();
                if (cnt < _rec)
                {
                    await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
                }
            }
        }
    }
}