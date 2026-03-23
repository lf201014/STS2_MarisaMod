using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace marisamod.Scripts.Cards
{
    public class ManaConvection : AbstractMarisaCard
    {
        public ManaConvection() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
        {
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new CardsVar(2),
        new EnergyVar(1),
        new DynamicVar("Draw",1)
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords => base.CanonicalKeywords.Concat([
            CardKeyword.Exhaust
        ]);

        protected override void OnUpgrade()
        {
            DynamicVars.Cards.UpgradeValueBy(1);
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            var draw = false;
            foreach (CardModel item in
            await CardSelectCmd.FromHand(prefs: new CardSelectorPrefs(SelectionScreenPrompt, 0, DynamicVars.Cards.IntValue), context: choiceContext, player: Owner, filter: null, source: this))
            {
                await CardCmd.Exhaust(choiceContext, item);
                await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
                if (item.DeckVersion == null)
                {
                    draw = true;
                }
            }
            if (draw)
                await CardPileCmd.Draw(choiceContext, DynamicVars["Draw"].BaseValue, Owner);
        }
    }
}