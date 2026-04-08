using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Cards;

// public class LuminesStrike : AbstractAmplifiedCard
// {
//     public LuminesStrike() : base(0, 1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
//     {
//     }
//
//     protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Concat([
//         new DynamicVar("Mult", 2),
//         new DynamicVar("MultAmp", 4),
//         new CalculationBaseVar(0m),
//         new ExtraDamageVar(1m),
//         new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) =>
//             card is AbstractAmplifiedCard { IsAmplified: true } ? card.Owner.PlayerCombatState!.Energy * card.DynamicVars["MultAmp"].IntValue : card.Owner.PlayerCombatState!.Hand.Cards.Count(x => x != card) * card.DynamicVars["Mult"].IntValue
//         )
//     ]);
//
//
//     protected override HashSet<CardTag> CanonicalTags =>
//     [
//         CardTag.Strike
//     ];
//
//     protected override void OnUpgrade()
//     {
//         DynamicVars["Mult"].UpgradeValueBy(1);
//         DynamicVars["MultAmp"].UpgradeValueBy(1);
//     }
//
//     protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
//     {
//         ArgumentNullException.ThrowIfNull(cardPlay.Target);
//         var cost = cardPlay.Resources.EnergySpent;
//         Log.Info($"LumineStrike.OnPlay: Cost: {cost}");
//         await DamageCmd.Attack(DynamicVars.CalculatedDamage).FromCard(this).Targeting(cardPlay.Target)
//             .WithHitFx("vfx/vfx_attack_slash")
//             .Execute(choiceContext);
//     }
// }
public class LuminesStrike : AbstractMarisaCard
{
    public LuminesStrike() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(0),
        new ExtraDamageVar(7),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) => card.Owner.PlayerCombatState!.Hand.Cards.Count(x => x.DeckVersion == null && x != card))
    ];

    protected override void OnUpgrade()
    {
        DynamicVars.ExtraDamage.UpgradeValueBy(3);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.CalculatedDamage).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }
}