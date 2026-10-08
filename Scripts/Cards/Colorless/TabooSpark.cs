using BaseLib.Utils;
using marisamod.Scenes.Vfx.HitVfx;
using marisamod.Scripts.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Cards;

[Pool(typeof(TokenCardPool))]
public class TabooSpark :AbstractAmplifiedCard
{
    public TabooSpark() : base(0, 1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars  =>
    [
        new DamageVar(6, ValueProp.Move),
        new CalculationBaseVar(6),
        new ExtraDamageVar(1),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((_, monster) => monster?.GetPowerAmount<HoukaiPower>() ?? 0),
        //new CardsVar(1)
    ];
    
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var damage = !AmplifiedInPlay ? DynamicVars.Damage.BaseValue : DynamicVars.CalculatedDamage.BaseValue;
        await DamageCmd.Attack(damage).FromCard(this).Targeting(cardPlay.Target)
            .WithHitVfxNode(t => SparkHitVfx.Create(NCombatRoom.Instance?.GetCreatureNode(t)!,"BurstSpark"))
            .BeforeDamage(async delegate
            {
                NSweepingBeamVfx nSweepingBeamVfx = NSweepingBeamVfx.Create(Owner.Creature, [cardPlay.Target])!;
                NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(nSweepingBeamVfx);
                await Cmd.Wait(0.5f);
            }).Execute(choiceContext);
    }
}