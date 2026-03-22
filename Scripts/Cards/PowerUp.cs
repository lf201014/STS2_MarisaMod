using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace marisamod.Scripts.Cards
{
    public class PowerUp : AbstractMarisaCard
    {
        public PowerUp() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("Add",2)
        ];

        protected override void OnUpgrade()
        {
            DynamicVars["Add"].UpgradeValueBy(1);
        }

        protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            foreach (CardModel card in Owner.PlayerCombatState!.Hand.Cards.Where(x => x.Type == CardType.Attack).ToArray())
            {
                if (card.DynamicVars.ContainsKey("CalculatedDamage"))
                {
                    card.DynamicVars.CalculationBase.UpgradeValueBy(DynamicVars["Add"].BaseValue);
                }
                else if (card.DynamicVars.ContainsKey("Damage"))
                {
                    card.DynamicVars.Damage.UpgradeValueBy(DynamicVars["Add"].BaseValue);
                }
            }

            return Task.CompletedTask;
        }
    }
}