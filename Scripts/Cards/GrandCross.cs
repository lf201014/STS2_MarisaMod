using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Cards;

public class GrandCross : AbstractMarisaCard
{
    public GrandCross() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(13m, ValueProp.Move),
        new EnergyVar(1)
    ];

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this,cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        // {
        //     return base.AfterCardPlayed(choiceContext, cardPlay);
        // }
        //
        // public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card is AbstractAmplifiedCard { AmplifiedInPlay: true } && cardPlay.Card.Owner == Owner)
        {
            EnergyCost.SetThisTurn(0);
        }

        return Task.CompletedTask;
    }
}