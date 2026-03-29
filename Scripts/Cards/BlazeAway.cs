using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace marisamod.Scripts.Cards
{
    public class BlazeAway : AbstractMarisaCard
    {
        public BlazeAway() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
        {
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new EnergyVar(1)
            ];


        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        protected override void OnUpgrade()
        {

            RemoveKeyword(CardKeyword.Exhaust);
        }


        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {

            var selection = (await CardSelectCmd.FromHand(prefs: new CardSelectorPrefs(base.SelectionScreenPrompt, 1), context: choiceContext, player: base.Owner, filter: delegate (CardModel c)
            {
                CardType type = c.Type;
                return (type == CardType.Attack) ? true : false;
            }, source: this)).FirstOrDefault();


            if (selection != null)
            {
                CardModel card = selection.CreateClone();
                await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, addedByPlayer: true);
                int energyGain = card.EnergyCost.GetWithModifiers(CostModifiers.All);
                //if (IsUpgraded)
                {
                    if (card is AbstractAmplifiedCard amplifiedCard && !amplifiedCard.CostModifiedForAmplify)
                    {
                        energyGain += amplifiedCard.KickerCost;
                    }
                }
                if (energyGain > 0)
                {
                    await PlayerCmd.GainEnergy(energyGain, Owner);
                }
            }
        }
    }
}