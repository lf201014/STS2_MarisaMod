using marisamod.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace marisamod.Scripts.Cards;

public class ViolentTricholoma : AbstractMarisaCard
{
    public ViolentTricholoma() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
    }
    
    //public override string PortraitPath => "res://marisamod/images/cards/marisamod-test_marisa_card.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2),
        new PowerVar<ChargeUpPower>(2)
    ];

    protected override void OnUpgrade()
    {
        DynamicVars["ChargeUpPower"].UpgradeValueBy(2);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        await PowerCmd.Apply<ChargeUpPower>(Owner.Creature, DynamicVars["ChargeUpPower"].BaseValue, Owner.Creature, this);
    }
}