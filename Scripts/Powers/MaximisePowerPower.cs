using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
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
            new DynamicVar("Mult", 50m)
        ];

        public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
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
            return (decimal)MathF.Pow(1.5f, Amount);
        }

        public override Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
        {
            if (power == this)
            {
                DynamicVars["Mult"].BaseValue = ((decimal)MathF.Pow(1.5f, Amount) - 1) * 100;
            }

            return Task.CompletedTask;
        }

        public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side == CombatSide.Player)
            {
                await PowerCmd.Remove(this);
            }
        }
    }
}