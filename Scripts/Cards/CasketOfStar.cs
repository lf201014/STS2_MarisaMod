using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using marisamod.Scripts.Cards.Colorless;
using marisamod.Scripts.Powers;

namespace marisamod.Scripts.Cards
{
    public class CasketOfStar : AbstractMarisaCard
    {
        public CasketOfStar() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
        {
        }

        //public override string PortraitPath => $"res://img/cards/CasketOfStar_p.png";

        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Spark>(IsUpgraded)];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (IsUpgraded)
            {
                await PowerCmd.Apply<CasketOfStarPlusPower>(Owner.Creature, 1m, Owner.Creature, this);
            }
            else
            {
                await PowerCmd.Apply<CasketOfStarPower>(Owner.Creature, 1m, Owner.Creature, this);
            }
        }
    }
}