using marisamod.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace marisamod.Scripts.Cards
{
    public class SatelliteIllusion : AbstractMarisaCard
    {
        public SatelliteIllusion() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) { }
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new EnergyVar(1)
        ];

        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Innate);
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<SatelliteIllusionPower>(Owner.Creature, 1, Owner.Creature, this);
        }
    }
}