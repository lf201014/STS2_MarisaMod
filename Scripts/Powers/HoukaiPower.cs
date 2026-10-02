using MegaCrit.Sts2.Core.Entities.Powers;

namespace marisamod.Scripts.Powers;

public class HoukaiPower : AbstractMarisaPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
}