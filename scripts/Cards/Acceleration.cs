using Godot;
using MarisaMod.scripts.Cards.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MarisaMod.scripts.Cards
{
    public class Acceleration : AbstractAmplifiedCard
    {
        public Acceleration() : base(0, 1, CardType.Skill, CardRarity.Common, TargetType.Self)
        {
        }

        public override string PortraitPath => $"res://img/cards/GuidingStar_p.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
                new CardsVar(1),
                new EnergyVar(1)
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            decimal draw = Mathf.Min(Owner?.PlayerCombatState?.DrawPile.Cards.Count ?? 0, 2);
            if (draw > 0 && Owner != null)
            {
                await CardPileCmd.Draw(choiceContext, draw, Owner);
                if (IsAmplified)
                    await CardPileCmd.Draw(choiceContext, draw, Owner);
            }
        }

        override protected void OnUpgrade()
        {
            DynamicVars.Cards.UpgradeValueBy(1);
        }
    }
}