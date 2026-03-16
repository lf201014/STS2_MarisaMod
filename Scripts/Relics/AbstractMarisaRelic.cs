using BaseLib.Abstracts;

namespace marisamod.Scripts.Relics;

public abstract class AbstractMarisaRelic : CustomRelicModel
{
    // 小图标
    public override string PackedIconPath => $"res://marisamod/images/relics/{Id.Entry.ToLowerInvariant()}.png";
    // 轮廓图标
    protected override string PackedIconOutlinePath => $"res://marisamod/images/relics/{Id.Entry.ToLowerInvariant()}.png";
    // 大图标
    protected override string BigIconPath => $"res://marisamod/images/relics/{Id.Entry.ToLowerInvariant()}.png";
}