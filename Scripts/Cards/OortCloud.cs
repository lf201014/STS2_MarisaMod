using marisamod.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace marisamod.Scripts.Cards
{
    public class OortCloud : AbstractAmplifiedCard
    {
        public OortCloud() : base(1, 1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
        {
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Concat([
            new CalculationBaseVar(5m),
            new CalculationExtraVar(2m),
            new CalculatedVar("Power").WithMultiplier((card, _) => card is AbstractAmplifiedCard { IsAmplified: true } ? 1 : 0),
        ]);

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<PlatingPower>()
        ];

        protected override void OnUpgrade()
        {
            DynamicVars.CalculationExtra.UpgradeValueBy(1);
            DynamicVars.CalculationBase.UpgradeValueBy(1);
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<PlatingPower>(Owner.Creature, (int)((CalculatedVar)base.DynamicVars["Power"]).Calculate(cardPlay.Target), Owner.Creature, this);
        }
    }
}