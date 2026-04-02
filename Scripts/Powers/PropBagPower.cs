using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace marisamod.Scripts.Powers;

public class PropBagPower : AbstractMarisaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    private static List<RelicModel> _relics = [];

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
}