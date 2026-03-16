using BaseLib.Extensions;
using Godot;
using marisamod.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

public class ChargeUpPower : AbstractMarisaPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

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
        if (Amount < 8)
        {
            return 1m;
        }
        return (decimal)Mathf.Pow(2, Mathf.FloorToInt(Amount / 8));
    }

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card != null && cardPlay.Card.Type == CardType.Attack && Amount >= 8)
        {
            decimal reduceAmount = Amount - Amount % 8;
            PowerCmd.ModifyAmount(this, -reduceAmount, Owner, null);
        }
        return base.AfterCardPlayed(context, cardPlay);
    }
}

