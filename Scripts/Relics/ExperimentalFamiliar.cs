using marisamod.Scripts.Cards.Colorless;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;

namespace marisamod.Scripts.Relics;

public class ExperimentalFamiliar : AbstractMarisaRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromCard<Spark>()
    ];

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == CombatSide.Player)
        {
            Flash();
            await CardPileCmd.AddGeneratedCardToCombat(combatState.CreateCard<Spark>(Owner), PileType.Hand, true);
        }
    }
}