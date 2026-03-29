using marisamod.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace marisamod.Scripts.Cards
{
    public class PolarisUnique : AbstractMarisaCard
    {
        public PolarisUnique() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new EnergyVar(2)
        ];

        protected override void OnUpgrade()
        {
            DynamicVars.Energy.UpgradeValueBy(1);
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
            //EnergyCost.AddThisCombat(1);
            await PowerCmd.Apply<PolarisUniquePower>(Owner.Creature, 1, Owner.Creature, this);
        }
    }
}