using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
// ReSharper disable PossibleLossOfFraction

namespace marisamod.Scripts.Cards;

public class GravityBeat : AbstractMarisaCard
{
    public GravityBeat() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(6m, ValueProp.Move),
        new DynamicVar("Div",12),
        new CalculationBaseVar(0),
        new CalculationExtraVar(1),
        new CalculatedVar("CalculatedRepeat").WithMultiplier((card,_)=>card.Owner.Deck.Cards.Count / card.DynamicVars["Div"].IntValue)
    ];

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
        DynamicVars["Div"].UpgradeValueBy(-2);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        var repeat = (int)((CalculatedVar)DynamicVars["CalculatedRepeat"]).Calculate(cardPlay.Target);
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).WithHitCount(repeat).FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitVfxNode((Creature t) => NStabVfx.Create(t, facingEnemies: true))
            .WithHitFx(null, null, "blunt_attack.mp3")
            .Execute(choiceContext);
        await CardPileCmd.Draw(choiceContext, repeat, Owner);
    }
}