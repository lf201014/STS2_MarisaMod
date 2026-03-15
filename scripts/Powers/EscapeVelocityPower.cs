using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Cards;

public class EscapeVelocityPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        if (player != Owner.Player)
        {
            return count;
        }
        if (AmountOnTurnStart == 0)
        {
            return count;
        }
        return count + (decimal)Amount * 2;
    }

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == Owner.Side && AmountOnTurnStart != 0)
        {
            await CardPileCmd.AddToCombatAndPreview<Burn>(Owner, PileType.Hand, Amount, addedByPlayer: false);
        }
    }
}