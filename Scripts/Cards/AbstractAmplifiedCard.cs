using marisamod.Scripts.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace marisamod.Scripts.Cards
{
    public abstract class AbstractAmplifiedCard(int baseCost, int kickerCost, CardType type, CardRarity rarity, TargetType target) : AbstractMarisaCard(baseCost, type, rarity, target)
    {
        public int KickerCost { get; } = kickerCost;

        public bool IsAmplified { get; protected set; }

        private bool _costModifiedForAmplify;

        public bool CostModifiedForAmplify => _costModifiedForAmplify;

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new EnergyVar(KickerCost)
        ];

        // public override IEnumerable<CardKeyword> CanonicalKeywords => [
        //     MarisaCardKeyWords.Amplify
        // ];

        public virtual void ValidateAmplify()
        {
            if (Owner.PlayerCombatState != null)
            {
                if (Owner.PlayerCombatState.Hand.Cards.Contains(this))
                {
                    if (Owner.Creature.HasPower<OneTimeOffPower>())
                    {
                        SetAmplifyState(false, false);
                    }
                    else if (Owner.Creature.HasPower<MillisecondPulsarsPower>() || Owner.Creature.HasPower<PulseMagicPower>())
                    {
                        SetAmplifyState(true, true);
                    }
                    else if (Owner.PlayerCombatState.Energy < EnergyCost.GetWithModifiers(CostModifiers.All))
                    {
                        SetAmplifyState(false, false);
                    }
                    else if (Owner.PlayerCombatState.Energy >= EnergyCost.GetWithModifiers(CostModifiers.All) + KickerCost)
                    {
                        SetAmplifyState(true, false);
                    }
                }
                else
                {
                    SetAmplifyState(false, false);
                }
            }
        }

        private void SetAmplifyState(bool isAmplified, bool costFree)
        {
            IsAmplified = isAmplified;
            if (isAmplified && !costFree && !_costModifiedForAmplify)
            {
                EnergyCost.AddThisCombat(KickerCost);
                _costModifiedForAmplify = true;
            }

            if (!isAmplified && _costModifiedForAmplify || costFree && _costModifiedForAmplify)
            {
                EnergyCost.AddThisCombat(-KickerCost);
                _costModifiedForAmplify = false;
            }
            //TODO CardText update
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

        public override Task AfterEnergyReset(Player player)
        {
            ValidateAmplify();
            return Task.CompletedTask;
        }

        public override Task AfterPotionUsed(PotionModel potion, Creature? target)
        {
            ValidateAmplify();
            return Task.CompletedTask;
        }
    }
}