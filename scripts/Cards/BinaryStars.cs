using marisamod.scripts.Cards.Abstract;
using marisamod.scripts.Cards.Colorless;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace marisamod.scripts.Cards
{
    public class BinaryStars : AbstractAmplifiedCard
    {
        public BinaryStars() : base(1, 1, CardType.Skill, CardRarity.Rare, TargetType.Self)
        {
        }


        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<BlackFlareStar>(), HoverTipFactory.FromCard<WhiteDwarf>()];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (IsAmplified)
            {
                List<CardModel> cards =
                [
                    CombatState!.CreateCard<BlackFlareStar>(Owner),
                    CombatState!.CreateCard<WhiteDwarf>(Owner)
                ];
                if (IsUpgraded)
                {
                    CardCmd.Upgrade(cards, CardPreviewStyle.HorizontalLayout);
                }
                await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, addedByPlayer: true);
            }
            else
            {
                List<CardModel> cards =
                [
                    ModelDb.Card<BlackFlareStar>(),
                    ModelDb.Card<WhiteDwarf>()
                ];
                if (IsUpgraded)
                {
                    CardCmd.Upgrade(cards, CardPreviewStyle.HorizontalLayout);
                }
                var cardModel = await CardSelectCmd.FromChooseACardScreen(choiceContext, cards, Owner, canSkip: true);
                if (cardModel != null)
                {
                    await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, addedByPlayer: true);
                }
            }
        }
    }
}