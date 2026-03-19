using BaseLib.Extensions;
using Godot;
using marisamod.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Powers
{
    public class ChargeUpPower : AbstractMarisaPower
    {
        public const int ChargeUpThreshold = 8;

        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Counter;

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DynamicVar("Mult", 1m)
        ];

        private bool _toBeConsumed = false;

        public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            if (!props.IsPoweredAttack_())
            {
                return 1m;
            }

            if (cardSource == null)
            {
                return 1m;
            }

            if (dealer != null && dealer != Owner && !Owner.Pets.Contains<Creature>(dealer))
            {
                return 1m;
            }

            //TODO temp threshold 8
            return CalculateMult();
        }

        public override Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
        {
            if (power == this)
            {
                DynamicVars["Mult"].BaseValue = CalculateMult();
            }

            return Task.CompletedTask;
        }

        public decimal CalculateMult()
        {
            //TODO temp threshold 8
            if (Amount < 8)
            {
                return 1m;
            }

            return (decimal)Mathf.Pow(2, Mathf.FloorToInt(Amount / 8f));
        }

        public override Task BeforeCardPlayed(CardPlay cardPlay)
        {
            if (cardPlay.Card.Type == CardType.Attack && Amount >= 8)
            {
                _toBeConsumed = true;
            }

            return Task.CompletedTask;
        }

        public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (_toBeConsumed)
            {
                decimal reduceAmount = Amount - Amount % 8;
                PowerCmd.ModifyAmount(this, -reduceAmount, Owner, null);
                _toBeConsumed = false;
            }
            _toBeConsumed = false;
            return Task.CompletedTask;
        }
    }

}