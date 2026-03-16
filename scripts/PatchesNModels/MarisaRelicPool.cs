using BaseLib.Abstracts;

namespace marisamod.Scripts.PatchesNModels;

public class MarisaRelicPool : CustomRelicPoolModel
{
    // 卡池的能量图标。加载路径为“res://images/atlases/ui_atlas.sprites/card/energy_{EnergyColorName}.tres”。
    public override string EnergyColorName => "marisa";
}
