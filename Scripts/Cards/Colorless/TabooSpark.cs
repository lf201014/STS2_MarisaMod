using BaseLib.Utils;
using marisamod.Scenes.Vfx.HitVfx;
using marisamod.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Cards.Colorless;

[Pool(typeof(TokenCardPool))]
public class TabooSpark : AbstractAmplifiedCard
{
    public TabooSpark() : base(0, 1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6, ValueProp.Move),
        new CalculationBaseVar(6),
        new ExtraDamageVar(1),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((_, monster) => monster?.GetPowerAmount<HoukaiPower>() ?? 0),
        //new CardsVar(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<HoukaiPower>()];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
        DynamicVars.CalculationBase.UpgradeValueBy(3);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var damage = !AmplifiedInPlay ? DynamicVars.Damage.BaseValue : DynamicVars.CalculatedDamage.BaseValue;
        await DamageCmd.Attack(damage).FromCard(this).Targeting(cardPlay.Target)
            .WithHitVfxNode(t => SparkHitVfx.Create(NCombatRoom.Instance?.GetCreatureNode(t)!, "BurstSpark"))
            .BeforeDamage(async delegate
            {
                var nSweepingBeamVfx = NSweepingBeamVfx.Create(Owner.Creature, [cardPlay.Target])!;
                NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(nSweepingBeamVfx);
                await Cmd.Wait(0.5f);
            }).Execute(choiceContext);
    }
}