using MarisaMod.scripts.Cards.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MarisaMod.scripts.Cards
{
    public class MillisecondPulsars : AbstractMarisaModCard
    {
        public MillisecondPulsars() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
        {
        }

        public override string PortraitPath => $"res://img/cards/MillisecondPulsars_p.png";

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<MillisecondPulsarsPower>(Owner.Creature, 1m, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Innate);
        }
    }
}