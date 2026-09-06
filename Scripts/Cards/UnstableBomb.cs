using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Cards;

public class UnstableBomb : AbstractAmplifiedCard //AbstractMarisaCard
{
    public UnstableBomb() : base(0, 1, CardType.Attack, CardRarity.Common, TargetType.RandomEnemy)
    {
    }

    //private static readonly int[] RandomPool = [0, 1];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        base.CanonicalVars.Concat([
            new DamageVar(1, ValueProp.Move),
            new DamageVar("DamageAmplified", 4, ValueProp.Move),
            new DamageVar("DamageOnPlay", 0, ValueProp.Move),
            new CalculationBaseVar(0),
            new ExtraDamageVar(1),
            new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) =>
                    card.DynamicVars["DamageOnPlay"].BaseValue
                //card.RunState!.Rng.CombatCardSelection.NextInt(card.DynamicVars.Damage.IntValue, card.DynamicVars["DamageAmplified"].IntValue + 1)
            ),
            new DynamicVar("RepeatBase", 2),
            new DynamicVar("RepeatAmp", 1)
        ]);

    protected override void OnUpgrade()
    {
        // DynamicVars["RepeatBase"].UpgradeValueBy(1);
        // DynamicVars["RepeatUpper"].UpgradeValueBy(1);
        DynamicVars.Damage.UpgradeValueBy(1);
        DynamicVars["DamageAmplified"].UpgradeValueBy(1);
        DynamicVars["RepeatAmp"].UpgradeValueBy(1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        // var hit = DynamicVars["RepeatBase"].IntValue +
        //           RandomPool.TakeRandom(1, RunState!.Rng.CombatCardSelection).FirstOrDefault();
        // var minDmg = DynamicVars.Damage.IntValue;
        // var maxDmg = DynamicVars["DamageAmplified"].IntValue;
        var hit = DynamicVars["RepeatBase"].IntValue;
        hit += AmplifiedInPlay ? DynamicVars["RepeatAmp"].IntValue : 0;
        // for (var i = 0; i < hit; i++)
        // {
        //     var damage = RunState!.Rng.CombatCardSelection.NextInt(minDmg, maxDmg + 1);
        //
        //     await DamageCmd.Attack(damage).FromCard(this,cardPlay)
        //         .TargetingRandomOpponents(CombatState!)
        //         .WithHitFx("vfx/vfx_attack_slash")
        //         .Execute(choiceContext);
        // }
        await DamageCmd.Attack(DynamicVars.CalculatedDamage).FromCard(this,cardPlay).WithHitCount(hit).BeforeDamage(() =>
            {
                DynamicVars["DamageOnPlay"].BaseValue = RunState!.Rng.CombatCardSelection.NextInt(DynamicVars.Damage.IntValue, DynamicVars["DamageAmplified"].IntValue + 1);
                return Task.CompletedTask;
            })
            .TargetingRandomOpponents(CombatState!)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }
}