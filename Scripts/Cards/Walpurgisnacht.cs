using marisamod.Scripts.Enchantments;
using marisamod.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace marisamod.Scripts.Cards;

public class Walpurgisnacht : AbstractAmplifiedCard //AbstractMarisaCard
{
    public Walpurgisnacht() : base(3, 1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }

    //public override string PortraitPath => "res://marisamod/images/cards/marisamod-test_marisa_card.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        base.CanonicalVars.Concat(
        [
            new DynamicVar("Power", 1),
            new DynamicVar("PowerAmp", 1)
        ]);

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        HoverTipFactory.FromEnchantment<StarlitEnchantment>().Concat([HoverTipFactory.FromPower<StarlitPower>()]);

    protected override void OnUpgrade()
    {
        DynamicVars["PowerAmp"].UpgradeValueBy(1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<WalpurgisnachtPower>(Owner.Creature, DynamicVars["Power"].IntValue, Owner.Creature, this);
        if (IsAmplified)
        {
            await PowerCmd.Apply<WalpurgisnachtPower>(Owner.Creature, DynamicVars["PowerAmp"].IntValue, Owner.Creature, this);
        }
    }
}