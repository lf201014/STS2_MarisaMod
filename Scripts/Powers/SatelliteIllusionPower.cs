using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace marisamod.Scripts.Powers
{

    public class SatelliteIllusionPower : AbstractMarisaPower
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Counter;

        private int _rec = 0;

        public override Task BeforeCardPlayed(CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner == Owner.Player)
            {
                _rec = Owner.Player.PlayerCombatState.DrawPile.Cards.Count;
            }
            return Task.CompletedTask;
        }

        public override async Task AfterCardPlayedLate(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner == Owner.Player)
            {
                var cnt = Owner.Player.PlayerCombatState.DrawPile.Cards.Count;
                if (cnt > _rec)
                {
                    await PlayerCmd.GainEnergy(Amount, Owner.Player);
                }
            }
        }
    }
}