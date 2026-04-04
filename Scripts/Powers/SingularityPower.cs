using marisamod.Scripts.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace marisamod.Scripts.Powers
{
    public class SingularityPower : AbstractMarisaPower
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Counter;

        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner == Owner.Player && cardPlay.Resources.EnergySpent == 0)
            {
                var card = Owner.Player.PlayerCombatState!.Hand.Cards.Where(x => x.Type == CardType.Attack).TakeRandom(1, Owner.Player.RunState.Rng.CombatCardSelection).FirstOrDefault();
                if (card != default)
                {
                    Flash();
                    if (card is AbstractMarisaCard marisaCard)
                        _ = marisaCard.DoFlash();
                    if (card.DynamicVars.ContainsKey("CalculatedDamage"))
                    {
                        card.DynamicVars.CalculationBase.UpgradeValueBy(Amount);
                    }
                    else if (card.DynamicVars.ContainsKey("Damage"))
                    {
                        card.DynamicVars.Damage.UpgradeValueBy(Amount);
                    }
                }
            }
        }
    }
}