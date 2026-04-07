// using MegaCrit.Sts2.Core.Combat;
// using MegaCrit.Sts2.Core.Commands;
// using MegaCrit.Sts2.Core.Entities.Cards;
// using MegaCrit.Sts2.Core.Entities.Players;
// using MegaCrit.Sts2.Core.GameActions.Multiplayer;
// using MegaCrit.Sts2.Core.Localization.DynamicVars;
// using MegaCrit.Sts2.Core.Models;
// using MegaCrit.Sts2.Core.ValueProps;
//
// namespace marisamod.Scripts.Cards;
//
// public class SuperPerseids : AbstractMarisaCard
// {
//     public SuperPerseids() : base(3, CardType.Attack, CardRarity.Uncommon, TargetType.RandomEnemy)
//     {
//     }
//
//     protected override IEnumerable<DynamicVar> CanonicalVars =>
//     [
//         new DamageVar(4, ValueProp.Move),
//         new EnergyVar(1),
//         new RepeatVar(3)
//     ];
//
//     protected override void OnUpgrade()
//     {
//         //DynamicVars.Damage.UpgradeValueBy(2);
//         DynamicVars.Repeat.UpgradeValueBy(1);
//     }
//
//     // public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
//
//     protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
//     {
//         await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(DynamicVars.Repeat.IntValue).FromCard(this)
//             .TargetingRandomOpponents(CombatState!)
//             .WithHitFx("vfx/vfx_attack_slash")
//             .Execute(choiceContext);
//     }
//
//     public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
//     {
//         if (card == this)
//         {
//             await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
//         }
//     }
//
//     public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
//     {
//         var pile = Pile;
//         if (pile is { Type: PileType.Exhaust } && player == Owner)
//         {
//             await CardPileCmd.Add(this, Owner.PlayerCombatState!.Hand);
//             DynamicVars.Damage.UpgradeValueBy(1);
//             EnergyCost.SetThisTurn(0);
//         }
//     }
//
//     // {
//     //     if (card == this)
//     //     {
//     //         await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(DynamicVars.Repeat.IntValue).FromCard(this)
//     //             .TargetingRandomOpponents(CombatState!)
//     //             .WithHitFx("vfx/vfx_attack_slash")
//     //             .Execute(choiceContext);
//     //     }
//     // }
//     // public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
// }