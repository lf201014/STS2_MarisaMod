using marisamod.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace marisamod.Scripts.Cards
{
    public class ChargingUp : AbstractMarisaCard
    {
        public ChargingUp() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
        {
        }

        //public override string PortraitPath => $"res://img/cards/ChargingUp_p.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Power", 6)];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<ChargeUpPower>()
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<ChargeUpPower>(Owner.Creature, DynamicVars["Power"].IntValue, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Power"].UpgradeValueBy(3);
        }
    }
}