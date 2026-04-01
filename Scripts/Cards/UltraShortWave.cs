using marisamod.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace marisamod.Scripts.Cards;

public class UltraShortWave : AbstractMarisaCard
{
    public UltraShortWave() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1),
        new DynamicVar("Power", 1),
        new DynamicVar("EnergyInc", 1),
        new DynamicVar("PowerInc", 1),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ChargeUpPower>()
    ];

    protected override void OnUpgrade()
    {
        DynamicVars["PowerInc"].UpgradeValueBy(1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
        await PowerCmd.Apply<ChargeUpPower>(Owner.Creature, DynamicVars["Power"].IntValue, Owner.Creature, this);

        DynamicVars.Energy.UpgradeValueBy(DynamicVars["EnergyInc"].IntValue);
        DynamicVars["Power"].UpgradeValueBy(DynamicVars["PowerInc"].IntValue);
    }
}