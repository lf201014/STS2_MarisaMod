using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace marisamod.Scripts.Powers;

public abstract class AbstractMarisaPower: CustomPowerModel
{
    public override PowerType Type { get; }
    public override PowerStackType StackType { get; }
    
    public override string CustomPackedIconPath => $"res://marisamod/images/powers/{Id.Entry.ToLowerInvariant()}.png";

    public override string CustomBigIconPath => $"res://marisamod/images/powers/{Id.Entry.ToLowerInvariant()}.png";

    public override string CustomBigBetaIconPath => $"res://marisamod/images/powers/{Id.Entry.ToLowerInvariant()}.png";
}