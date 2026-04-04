using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Cards;

public class CollectingQuirk : AbstractMarisaCard
{
    public CollectingQuirk() : base(2, CardType.Attack, CardRarity.Rare, TargetType.RandomEnemy)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9m, ValueProp.Move),
        new DynamicVar("Div", 4),
        new CalculationBaseVar(0),
        new CalculationExtraVar(1m),
        new CalculatedVar("CalculatedHits").WithMultiplier((card, _) => 
            Mathf.FloorToInt(card.Owner.Relics.Count / (float)card.DynamicVars["Div"].IntValue))
    ];

    protected override void OnUpgrade()
    {
        DynamicVars["Div"].UpgradeValueBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount((int)((CalculatedVar)DynamicVars["CalculatedHits"]).Calculate(null)).FromCard(this)
            .TargetingRandomOpponents(CombatState!)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }
}