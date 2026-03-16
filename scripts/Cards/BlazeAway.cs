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

        private CardModel? _cardSource;

        //public override string PortraitPath => $"res://img/cards/blazeAway_p.png";
        protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

        //TODO CardPreview

        protected override void OnUpgrade()
        {
            DynamicVars.Cards.UpgradeValueBy(1);
        }

        protected override IEnumerable<IHoverTip> ExtraHoverTips => _cardSource != null ? [HoverTipFactory.FromCard(_cardSource)] : [];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (_cardSource == null)
            {
                GetLastAttackForTurn();
            }

            if (_cardSource != null)
            {
                await CardCmd.AutoPlay(choiceContext, _cardSource.CreateDupe(), null);
            }
        }

        public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (cardPlay.Card != this && cardPlay.Card.Type == CardType.Attack && cardPlay.Card.Owner == Owner)
            {
                _cardSource = cardPlay.Card;
            }

            return base.AfterCardPlayed(context, cardPlay);
        }

        public override Task AfterCardEnteredCombat(CardModel card)
        {
            if (card == this)
            {
                GetLastAttackForTurn();
            }

            return base.AfterCardEnteredCombat(card);
        }

        private void GetLastAttackForTurn()
        {
            CardModel? res = CombatManager.Instance.History.CardPlaysFinished.LastOrDefault(e =>
                e.HappenedThisTurn(Owner.Creature.CombatState) && e.CardPlay.Card.Type == CardType.Attack && e.CardPlay.Card.Owner == Owner)?.CardPlay.Card;
            _cardSource = res;
        }
    }
}