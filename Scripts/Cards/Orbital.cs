using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace marisamod.Scripts.Cards
{
    public class Orbital : AbstractMarisaCard
    {
        public Orbital() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
        {
        }

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DynamicVar("Add", 1),
            new DynamicVar("Draw", 1)
        ];

        protected override void OnUpgrade()
        {
            DynamicVars["Add"].UpgradeValueBy(1);
        }

        public override IEnumerable<CardKeyword> CanonicalKeywords => base.CanonicalKeywords.Concat([
            CardKeyword.Exhaust
        ]);

        protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            return Task.CompletedTask;
        }

        public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
        {
            if (card == this)
            {
                await CardPileCmd.Draw(choiceContext, DynamicVars["Draw"].IntValue, Owner);
            }
        }

        public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
        {
            if (card == this)
            {
                var prefs = new CardSelectorPrefs(SelectionScreenPrompt, DynamicVars["Add"].IntValue);
                var pile = PileType.Exhaust.GetPile(Owner).Cards.Where(x => x is not Orbital).ToArray();
                var cards = (await CardSelectCmd.FromSimpleGrid(choiceContext, pile, Owner, prefs)).ToArray();
                if (cards.Length > 0)
                {
                    foreach (CardModel item in cards)
                    {
                        await CardPileCmd.Add(item, PileType.Hand);
                    }
                }
            }
        }
    }
}