using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace marisamod.Scripts.Powers;

public class PrismaticNebularPower : AbstractMarisaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == Owner.Side)
        {
            Flash();
            await PowerCmd.Remove(this);
        }
    }

    public override decimal ModifyPowerAmountGivenAdditive(PowerModel power, Creature giver, decimal amount, Creature? target, CardModel? cardSource)
    {
        if (power is StarlitPower && giver == Owner && amount > 0)
        {
            var enumerable = CombatState.GetTeammatesOf(Owner).Where(c => c is { IsAlive: true, IsPlayer: true } && c != Owner);
            foreach (var creature in enumerable)
            {
                _ = PowerCmd.Apply<StarlitPower>(new ThrowingPlayerChoiceContext(), creature, amount + Amount - 1, Owner, null);
            }
        }

        return base.ModifyPowerAmountGivenAdditive(power, giver, amount, target, cardSource);
    }
}