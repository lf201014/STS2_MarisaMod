using marisamod.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace marisamod.Scripts.Cards;

public class OrrerysGalaxy : AbstractMarisaCard
{
    public OrrerysGalaxy() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Mult", 2)
    ];
    
    //public override string PortraitPath => "res://marisamod/images/cards/marisamod-test_marisa_card.png";

    protected override void OnUpgrade()
    {
        DynamicVars["Mult"].UpgradeValueBy(1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var amt = Owner.Creature.GetPowerAmount<StarlitPower>();
        if (amt > 0)
        {
            amt *= DynamicVars["Mult"].IntValue - 1;
            await PowerCmd.Apply<StarlitPower>(Owner.Creature, amt, Owner.Creature, this);
        }
    }
}