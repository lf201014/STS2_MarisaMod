using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
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
    public class MaximisePowerPower : AbstractMarisaPower
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Single;

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DynamicVar("Mult", 200)
        ];

        public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource,CardPlay? cardPlay)
        {
            if (dealer != Owner && dealer != null && !Owner.Pets.Contains(dealer))
            {
                return 1m;
            }

            if (!props.IsPoweredAttack_())
            {
                return 1m;
            }

            if (cardSource == null)
            {
                return 1m;
            }

            //return (decimal)MathF.Pow(1.5f, Amount);
            return (decimal)MathF.Pow(2, Amount);
        }

        public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
        {
            if (power == this)
            {
                //DynamicVars["Mult"].BaseValue = (decimal)MathF.Pow(1.5f, Amount) * 100;
                DynamicVars["Mult"].BaseValue = (decimal)MathF.Pow(2f, Amount) * 100;
            }

            return Task.CompletedTask;
        }

        public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            if (side == Owner.Side)
            {
                await PowerCmd.Remove(this);
            }
        }
    }
}