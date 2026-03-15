using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MarisaMod.scripts.PatchesNModels;

namespace MarisaMod.scripts.Cards.Abstract
{
    public abstract class AbstractAmplifiedCard : AbstractMarisaModCard
    {
        public int KickerCost { get; protected set; }

        public bool IsAmplified { get; protected set; }

        protected AbstractAmplifiedCard(int baseCost, int kickerCost, CardType type, CardRarity rarity, TargetType target, bool showInCardLibrary = true, bool autoAdd = true) 
        : base(baseCost, type, rarity, target, showInCardLibrary, autoAdd)
        {
            KickerCost = kickerCost;
        }

        public override IEnumerable<CardKeyword> CanonicalKeywords => [
            CustomKeywords.Amplify
        ];

        public virtual void ValidateAmplify()
        {
            if (Owner != null && Owner.PlayerCombatState != null)
            {
                if (Owner.Creature.HasPower<PulseMagicePower>() || Owner.Creature.HasPower<MillisecondPulsarsPower>())
                {
                    IsAmplified = true;
                    return;
                }

                if (Owner.PlayerCombatState.Hand.Cards.Contains(this))
                {
                    if (IsAmplified && Owner.PlayerCombatState.Energy < EnergyCost.GetWithModifiers(CostModifiers.All))
                    {
                        IsAmplified = false;
                        EnergyCost.AddThisTurnOrUntilPlayed(-KickerCost);
                        //TODO CardText update
                    }
                    if (!IsAmplified && Owner.PlayerCombatState.Energy >= EnergyCost.GetWithModifiers(CostModifiers.All) + KickerCost)
                    {
                        IsAmplified = true;
                        EnergyCost.AddThisTurnOrUntilPlayed(KickerCost);
                        //TODO CardText update
                    }
                }
            }
        }

        public override Task AfterCardEnteredCombat(CardModel card)
        {
            if (card == this)
                ValidateAmplify();
            return base.AfterCardEnteredCombat(card);
        }

        public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            ValidateAmplify();
            return base.AfterCardPlayed(context, cardPlay);
        }

        public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
        {
            if (card == this)
                ValidateAmplify();
            return base.AfterCardDrawn(choiceContext, card, fromHandDraw);
        }
    }
}