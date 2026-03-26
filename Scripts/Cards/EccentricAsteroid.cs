using marisamod.Scripts.PatchesNModels;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Cards;

public class EccentricAsteroid : AbstractMarisaCard
{
    public EccentricAsteroid() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
    }
    
    public override string PortraitPath => "res://marisamod/images/cards/marisamod-test_marisa_card.png";

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(6),
        new CalculationExtraVar(2),
        new CalculatedBlockVar(ValueProp.Move).WithMultiplier((card, _) =>
            CombatManager.Instance.History.CardPlaysFinished.Count(e =>
                e.HappenedThisTurn(card.CombatState) && e.CardPlay.Card.Tags.Contains(MarisaCardTags.Spark) && e.CardPlay.Card.Owner == card.Owner))
    ];

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(2);
        DynamicVars.CalculationExtra.UpgradeValueBy(1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.CalculatedBlock.Calculate(cardPlay.Target), DynamicVars.CalculatedBlock.Props, cardPlay);
    }
}