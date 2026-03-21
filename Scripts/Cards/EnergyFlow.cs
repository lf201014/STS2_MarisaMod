using marisamod.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace marisamod.Scripts.Cards;

public class EnergyFlow : AbstractMarisaCard
{
    public EnergyFlow() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<EnergyFlowPower>(2)
    ];

    protected override void OnUpgrade()
    {
        DynamicVars["EnergyFlowPower"].UpgradeValueBy(1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<EnergyFlowPower>(Owner.Creature, DynamicVars["EnergyFlowPower"].BaseValue, Owner.Creature, this);
    }
}