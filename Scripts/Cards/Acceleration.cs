using Godot;
using marisamod.scripts.Cards.Abstract;
using marisamod.Scripts.Cards.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace marisamod.scripts.Cards
{
    public class Acceleration : AbstractMarisaCard
    {
        public Acceleration() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
        {
        }

        //public override string PortraitPath => $"res://img/cards/GuidingStar_p.png";

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new CardsVar(3)
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            decimal draw = Mathf.Min(Owner?.PlayerCombatState?.DrawPile.Cards.Count ?? 0, 2);
            if (draw > 0 && Owner != null)
            {
                await CardPileCmd.Draw(choiceContext, draw, Owner);
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Cards.UpgradeValueBy(1);
        }
    }
}