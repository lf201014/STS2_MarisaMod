using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Powers;

public class StarlitPower : AbstractMarisaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("AoeEot", 1),
        new DynamicVar("BlockEot", 2)
    ];


    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == Owner.Side)
        {
            CalculateVars();
            await CreatureCmd.GainBlock(Owner, DynamicVars["BlockEot"].IntValue, ValueProp.Unpowered, null);
            await CreatureCmd.Damage(choiceContext, CombatState.HittableEnemies, DynamicVars["DamageEot"].IntValue, ValueProp.Unpowered, Owner);
            if (!Owner.HasPower<SprinkleStarNHeartPower>())
                await PowerCmd.Remove(this);
        }
    }

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        CalculateVars();
        return Task.CompletedTask;
    }

    public override Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        CalculateVars();
        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        CalculateVars();
        return Task.CompletedTask;
    }

    private void CalculateVars()
    {
        DynamicVars["AoeEot"].BaseValue = Amount * (1 + Owner.GetPowerAmount<OrrerysGalaxyDamagePower>());
        DynamicVars["BlockEot"].BaseValue = Amount * (2 + Owner.GetPowerAmount<OrrerysGalaxyBlockPower>());
    }
}