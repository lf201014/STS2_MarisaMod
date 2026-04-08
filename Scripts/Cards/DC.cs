using marisamod.Scripts.Characters;
using marisamod.Scripts.Enchantments;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using static MegaCrit.Sts2.Core.Models.ModelDb;

namespace marisamod.Scripts.Cards;

public class DC : AbstractMarisaCard
{
    //     public DC() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    //     {
    //     }
    //
    //     public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Innate];
    //
    //     protected override IEnumerable<DynamicVar> CanonicalVars => [
    //         // new CalculationBaseVar(5m),
    //         // new ExtraDamageVar(5m),
    //         // new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _)=>card.Owner.PlayerCombatState!.DiscardPile.Cards.Count == 0 ? 1 : 0)
    //         new DamageVar(5,ValueProp.Move)
    //         ];
    //
    //     protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    //     {
    //         ArgumentNullException.ThrowIfNull(cardPlay.Target);
    //         var repeat = Owner.PlayerCombatState!.DiscardPile.Cards.Count == 0 ? 2 : 1;
    //         await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(repeat).FromCard(this).Targeting(cardPlay.Target)
    //             .WithHitFx("vfx/vfx_attack_slash")
    //             .Execute(choiceContext);
    //     }
    //
    //     protected override void OnUpgrade()
    //     {
    //         // DynamicVars.CalculationBase.UpgradeValueBy(2m);
    //         // DynamicVars.ExtraDamage.UpgradeValueBy(2m);
    //         DynamicVars.Damage.UpgradeValueBy(2);
    //     }
    public DC() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8, ValueProp.Move),
        new CardsVar(2)
    ];

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        var zaEnchantment = Enchantment<StarlitEnchantment>().ToMutable();
        var cards = Owner.PlayerCombatState!.Hand.Cards.ToList();
        cards = cards.Where(x => zaEnchantment.CanEnchant(x)).ToList();
        if (cards.Count > DynamicVars.Cards.IntValue)
        {
            cards = cards.TakeRandom(DynamicVars.Cards.IntValue, RunState!.Rng.CombatCardSelection).ToList();
        }

        foreach (var card in cards)
        {
            MarisaCharacter.Enchant(zaEnchantment, card);
        }
    }
}