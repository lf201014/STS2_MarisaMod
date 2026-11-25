using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Rooms;
using static MegaCrit.Sts2.Core.HoverTips.HoverTipFactory;

namespace marisamod.Scripts.Powers;

public class PropBagPower : AbstractMarisaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private List<RelicModel> _relics = [];

    protected override void AfterCloned()
    {
        _relics = [];
    }
        

    public override int DisplayAmount => _relics.Count;

    public void ClearRelicList()
    {
        _relics.Clear();
    }

    public void AddRelicToList(RelicModel relic)
    {
        _relics.Add(relic);
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        foreach (var relic in _relics)
        {
            await RelicCmd.Remove(relic);
        }

        ClearRelicList();
    }

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature,CardModel? source, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (wasRemovalPrevented || creature != Owner)
        {
            return;
        }
        foreach (var relic in _relics)
        {
            await RelicCmd.Remove(relic);
        }

        ClearRelicList();
    }
    protected override IEnumerable<IHoverTip> ExtraHoverTips => _relics.Select(c => (IHoverTip)new HoverTip(c.Title));
}