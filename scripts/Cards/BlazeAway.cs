using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MarisaMod.scripts.Cards.Abstract;

namespace MarisaMod.scripts.Cards
{
    public class BlazeAway : AbstractMarisaModCard
    {
        public BlazeAway() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
        {
        }

        CardModel? cardSource = null;

        public override string PortraitPath => $"res://img/cards/blazeAway_p.png";
        protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

        //TODO CardPreview

        override protected void OnUpgrade()
        {
            DynamicVars.Cards.UpgradeValueBy(1);
        }

        protected override IEnumerable<IHoverTip> ExtraHoverTips => cardSource != null ? [HoverTipFactory.FromCard(cardSource)] : [];

        override protected async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (cardSource == null)
            {
                GetLastAttackForTurn();
            }
            if (cardSource != null)
            {
                await CardCmd.AutoPlay(choiceContext, cardSource.CreateDupe(), null);
            }
        }

        override public Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (cardPlay.Card != this && cardPlay.Card.Type == CardType.Attack && cardPlay.Card.Owner == Owner)
            {
                cardSource = cardPlay.Card;
            }
            return base.AfterCardPlayed(context, cardPlay);
        }

        override public Task AfterCardEnteredCombat(CardModel card)
        {
            if (card == this)
            {
                GetLastAttackForTurn();
            }
            return base.AfterCardEnteredCombat(card);
        }

        private void GetLastAttackForTurn()
        {
            CardModel? res = CombatManager.Instance.History.CardPlaysFinished.LastOrDefault((CardPlayFinishedEntry e) =>
             e.HappenedThisTurn(Owner.Creature.CombatState) && e.CardPlay.Card.Type == CardType.Attack && e.CardPlay.Card.Owner == Owner)?.CardPlay.Card;
            cardSource = res;
        }
    }
}