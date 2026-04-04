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

    private static readonly int[] RandomPool = [0, 1];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        base.CanonicalVars.Concat([
            new DamageVar(2, ValueProp.Move),
            new DynamicVar("RepeatBase", 2),
            new DynamicVar("RepeatUpper", 3),
            new DynamicVar("RepeatAmp", 1)
        ]);

    protected override void OnUpgrade()
    {
        // DynamicVars["RepeatBase"].UpgradeValueBy(1);
        // DynamicVars["RepeatUpper"].UpgradeValueBy(1);
        DynamicVars.Damage.UpgradeValueBy(1);
        DynamicVars["RepeatAmp"].UpgradeValueBy(1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var hit = DynamicVars["RepeatBase"].IntValue +
                  RandomPool.TakeRandom(1, RunState!.Rng.CombatCardSelection).FirstOrDefault();

        hit += IsAmplified ? DynamicVars["RepeatAmp"].IntValue : 0;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(hit).FromCard(this)
            .TargetingRandomOpponents(CombatState!)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }
}