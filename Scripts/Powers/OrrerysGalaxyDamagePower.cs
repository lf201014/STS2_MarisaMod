using MegaCrit.Sts2.Core.Entities.Powers;

namespace marisamod.Scripts.Powers;

public class OrrerysGalaxyDamagePower : AbstractMarisaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}