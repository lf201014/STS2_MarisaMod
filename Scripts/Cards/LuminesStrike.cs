using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
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
//         await DamageCmd.Attack(DynamicVars.CalculatedDamage).FromCard(this,cardPlay).Targeting(cardPlay.Target)
//             .WithHitFx("vfx/vfx_attack_slash")
//             .Execute(choiceContext);
//     }
// }
// public class LuminesStrike : AbstractMarisaCard
// {
//     public LuminesStrike() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
//     {
//     }
//
//     protected override IEnumerable<DynamicVar> CanonicalVars =>
//     [
//         new CalculationBaseVar(7),
//         new ExtraDamageVar(4),
//         new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) => card.Owner.PlayerCombatState!.Hand.Cards.Count(x => x.DeckVersion == null && x != card))
//     ];
//
//     protected override void OnUpgrade()
//     {
//         DynamicVars.CalculationBase.UpgradeValueBy(2);
//         DynamicVars.ExtraDamage.UpgradeValueBy(2);
//     }
//
//     protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
//     {
//         ArgumentNullException.ThrowIfNull(cardPlay.Target);
//         await DamageCmd.Attack(DynamicVars.CalculatedDamage).FromCard(this,cardPlay).Targeting(cardPlay.Target)
//             .WithHitFx("vfx/vfx_attack_slash")
//             .Execute(choiceContext);
//     }
// }

public class LuminesStrike : AbstractMarisaCard
{
    public LuminesStrike() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllAllies)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(6),
        new ExtraDamageVar(6),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) => CombatManager.Instance.History.Entries.Count(x => x is CardGeneratedEntry { Card: Burn })),
        new CardsVar(1)
    ];

    protected override void OnUpgrade()
    {
        DynamicVars.ExtraDamage.UpgradeValueBy(2m);
        DynamicVars.CalculationBase.UpgradeValueBy(2m);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.CalculatedDamage).FromCard(this,cardPlay).TargetingAllOpponents(CombatState!)
            .WithHitFx("vfx/vfx_attack_blunt", null, "heavy_attack.mp3")
            .Execute(choiceContext);
        await CardPileCmd.AddGeneratedCardToCombat(CombatState!.CreateCard<Burn>(Owner), PileType.Hand, Owner);
    }
}