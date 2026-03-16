using BaseLib.Abstracts;
using Godot;

namespace marisamod.Scripts.PatchesNModels;

public class MarisaCardPool : CustomCardPoolModel
{
    // 卡池的ID。必须唯一防撞车。
    public override string Title => "test";

    // 卡池的能量图标。暂时不支持加载，建议暂时使用原版的。
    public override string EnergyColorName => "defect";

    // 卡池的主题色。通常是卡牌框架的颜色。
    public override Color DeckEntryCardColor => new(0.5f, 0.5f, 1f);

    // 卡池是否是无色。例如事件、状态等卡池就是无色的。
    public override bool IsColorless => false;
}
