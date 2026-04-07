using marisamod.Scripts.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace marisamod.Scripts.Powers;

public class DeepEcologicalBombPower : AbstractMarisaPower, ITemporaryPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;


    public override LocString Description => new("powers", "TEMPORARY_STRENGTH_DOWN.description");

    protected override string SmartDescriptionLocKey => "TEMPORARY_STRENGTH_DOWN.smartDescription";


    private bool _shouldIgnoreNextInstance;

    public override async Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (_shouldIgnoreNextInstance)
        {
            _shouldIgnoreNextInstance = false;
        }
        else
        {
            await PowerCmd.Apply<StrengthPower>(target, -amount, applier, cardSource, silent: true);
        }
    }

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (amount != Amount && power == this)
        {
            if (_shouldIgnoreNextInstance)
            {
                _shouldIgnoreNextInstance = false;
            }
            else
            {
                await PowerCmd.Apply<StrengthPower>(Owner, -amount, applier, cardSource, silent: true);
            }
        }
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == Owner.Side)
        {
            Flash();
            await PowerCmd.Remove(this);
            await PowerCmd.Apply<StrengthPower>(Owner, Amount, Owner, null);
        }
    }

    public void IgnoreNextInstance()
    {
        _shouldIgnoreNextInstance = true;
    }

    public AbstractModel OriginModel => ModelDb.Card<DeepEcologicalBomb>();
    public PowerModel InternallyAppliedPower => ModelDb.Power<StrengthPower>();
}