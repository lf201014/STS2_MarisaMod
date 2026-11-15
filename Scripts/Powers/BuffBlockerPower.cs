using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace marisamod.Scripts.Powers;

public class BuffBlockerPower : AbstractMarisaPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override decimal ModifyPowerAmountGivenMultiplicative(PowerModel power, Creature giver, decimal amount, Creature? target, CardModel? cardSource)
    {
        if (power.GetTypeForAmount(amount) == PowerType.Buff && target == Owner && power is not PropBagPower)
        {
            return 0;
        }

        return base.ModifyPowerAmountGivenMultiplicative(power, giver, amount, target, cardSource);
    }

    public override async Task BeforeSideTurnEndVeryEarly(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}