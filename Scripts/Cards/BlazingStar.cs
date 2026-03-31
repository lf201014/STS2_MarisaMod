using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Cards
{
    public class BlazingStar : AbstractAmplifiedCard
    {
        // public BlazingStar() : base(2, 1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
        // {
        // }


        // protected override IEnumerable<DynamicVar> CanonicalVars =>
        // [
        //     new CalculationBaseVar(16m),
        //     new ExtraDamageVar(8m),
        //     new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) =>
        //         // card is AbstractAmplifiedCard { IsAmplified: true }
        //         //     ? card.Owner.PlayerCombatState!.Hand.Cards.Count(IsBurn) * 2 + 2 :
        //         card.Owner.PlayerCombatState!.Hand.Cards.Count(IsBurn)),
        //     new RepeatVar(2),
        //     new EnergyVar(1)
        // ];

        // protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        // {
        //     ArgumentNullException.ThrowIfNull(cardPlay.Target);
        //     var repeat = IsAmplified ? DynamicVars.Repeat.IntValue : 1;
        //     //await DamageCmd.Attack(DynamicVars.CalculatedDamage).FromCard(this).Targeting(cardPlay.Target)
        //     await DamageCmd.Attack(DynamicVars.CalculatedDamage).WithHitCount(repeat).FromCard(this).Targeting(cardPlay.Target)
        //         .WithHitFx("vfx/vfx_attack_blunt", null, "blunt_attack.mp3")
        //         .Execute(choiceContext);
        // }

        // protected override void OnUpgrade()
        // {
        //     DynamicVars.CalculationBase.UpgradeValueBy(4m);
        //     DynamicVars.ExtraDamage.UpgradeValueBy(2m);
        // }

        // private static bool IsBurn(CardModel card)
        // {
        //     return card is Burn;
        // }

        public BlazingStar() : base(1, 2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Concat([
            new DamageVar(15,ValueProp.Move),
            new CardsVar(1)
        ]);

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(5);
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target);
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
            await CardPileCmd.AddGeneratedCardToCombat(CombatState!.CreateCard<Burn>(Owner), PileType.Hand, addedByPlayer: true);

            if (IsAmplified)
            {
                //AddKeyword(CardKeyword.Exhaust);
                await CreatureCmd.Stun(cardPlay.Target);
                await CardCmd.Exhaust(choiceContext, this);
            }
        }
    }
}