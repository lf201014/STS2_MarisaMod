using marisamod.scripts.PatchesNModels;
using marisamod.scripts.Powers;
using marisamod.Scripts.Cards.Abstract;
using marisamod.Scripts.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace marisamod.scripts.Cards.Abstract
{
    public abstract class AbstractAmplifiedCard(int baseCost, int kickerCost, CardType type, CardRarity rarity, TargetType target, bool showInCardLibrary = true, bool autoAdd = true)
        : AbstractMarisaCard(baseCost, type, rarity, target, showInCardLibrary)
    {
        public int KickerCost { get; } = kickerCost;

        public bool IsAmplified { get; protected set; }
        private bool _costModifiedForAmplify;

        public override IEnumerable<CardKeyword> CanonicalKeywords => [
            MarisaCardKeyWords.Amplify
        ];


        public virtual void ValidateAmplify()
        {
            if (Owner.PlayerCombatState != null)
            {
                if (Owner.PlayerCombatState.Hand.Cards.Contains(this))
                {
                    if (Owner.Creature.HasPower<MillisecondPulsarsPower>() || Owner.Creature.HasPower<PulseMagicePower>())
                    {
                        IsAmplified = true;
                        if (_costModifiedForAmplify)
                        {
                            EnergyCost.AddThisCombat(-KickerCost);
                            _costModifiedForAmplify = false;
                        }
                    }


                    if (IsAmplified && Owner.PlayerCombatState.Energy < EnergyCost.GetWithModifiers(CostModifiers.All))
                    {
                        IsAmplified = false;
                        _costModifiedForAmplify = true;
                        EnergyCost.AddThisCombat(-KickerCost);
                        //TODO CardText update
                    }

                    if (!IsAmplified && Owner.PlayerCombatState.Energy >= EnergyCost.GetWithModifiers(CostModifiers.All) + KickerCost)
                    {
                        IsAmplified = true;                        
                        _costModifiedForAmplify = false;
                        EnergyCost.AddThisCombat(KickerCost);
                        //TODO CardText update
                    }
                }
            }
        }

        public override Task AfterCardEnteredCombat(CardModel card)
        {
            if (card == this)
                ValidateAmplify();
            return Task.CompletedTask;
        }

        public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            ValidateAmplify();
            return Task.CompletedTask;
        }

        public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
        {
            if (card == this)
                ValidateAmplify();
            return Task.CompletedTask;
        }
    }
}