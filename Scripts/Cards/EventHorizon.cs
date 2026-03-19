using marisamod.Scripts.Cards.Abstract;
using marisamod.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace marisamod.Scripts.Cards;

public class EventHorizon : AbstractMarisaCard
{
    public EventHorizon() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>[
        new DynamicVar("Power",1)
    ];

    protected override void OnUpgrade()
    {
        DynamicVars["Power"].UpgradeValueBy(1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<EventHorizonPower>(Owner.Creature, DynamicVars.Cards.BaseValue, Owner.Creature, this);
    }
}