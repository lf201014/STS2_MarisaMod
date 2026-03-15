using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;

public class MillisecondPulsarsPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;
}