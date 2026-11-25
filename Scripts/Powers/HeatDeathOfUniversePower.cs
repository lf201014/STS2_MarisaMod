using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace marisamod.Scripts.Powers;

public class HeatDeathOfUniversePower : AbstractMarisaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public Creature PowerTarget;

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != Owner.Side)
        {
            return;
        }

        if (Owner.Player != null)
        {
            var sum = 0;
            foreach (var item in await CardSelectCmd.FromHand(choiceContext, Owner.Player, new CardSelectorPrefs(SelectionScreenPrompt, 0, 99), null, this))
            {
                await CardCmd.Exhaust(choiceContext, item);
                sum += 1;
            }

            if (PowerTarget != null)
                await PowerCmd.Apply<ChargeUpPower>(choiceContext, PowerTarget, sum, Owner, null);
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}