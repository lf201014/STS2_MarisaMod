using marisamod.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace marisamod.Scripts.Cards;

public class BoundOfProtagonists : AbstractMarisaCard
{
    public BoundOfProtagonists() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }
    
    public override string PortraitPath => "res://marisamod/images/cards/marisamod-test_marisa_card.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Power", 2)
    ];

    protected override void OnUpgrade()
    {
        DynamicVars["Power"].UpgradeValueBy(1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<FlightPower>(Owner.Creature, DynamicVars["Power"].IntValue, Owner.Creature, this);
    }
}