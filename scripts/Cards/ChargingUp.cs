using MarisaMod.scripts.Cards.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MarisaMod.scripts.Cards
{
    public class ChargingUp : AbstractMarisaModCard
    {
        public ChargingUp() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
        {
        }

        public override string PortraitPath => $"res://img/cards/ChargingUp_p.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Power", 5)];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

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