using marisamod.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace marisamod.Scripts.Cards
{
    public class Singularity : AbstractMarisaCard
    {
        public Singularity() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("Power",2)
            ];

        protected override void OnUpgrade()
        {
            DynamicVars["Power"].UpgradeValueBy(1);
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<SingularityPower>(Owner.Creature, DynamicVars["Power"].IntValue, Owner.Creature, this);
        }
    }
}