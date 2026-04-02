using marisamod.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace marisamod.Scripts.Cards;

public class SprinkleStarNHeart : AbstractAmplifiedCard
{
    public SprinkleStarNHeart() : base(0, 1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    //public override string PortraitPath => "res://marisamod/images/cards/marisamod-test_marisa_card.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Concat([
        new DynamicVar("Power", 3)
    ]);

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StarlitPower>()
    ];

    protected override void OnUpgrade()
    {
        DynamicVars["Power"].UpgradeValueBy(1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<SprinkleStarNHeartPower>(Owner.Creature, 1, Owner.Creature, this);
        if (IsAmplified)
        {
            await PowerCmd.Apply<StarlitPower>(Owner.Creature, DynamicVars["Power"].IntValue, Owner.Creature, this);
        }
    }
}