using BaseLib.Abstracts;
using Godot;

namespace marisamod.Scripts.PatchesNModels;

public class MarisaCardPool : CustomCardPoolModel, ICustomEnergyIconPool
{
    // 卡池的ID。必须唯一防撞车。
    public override string Title => "marisa";

    //public override string EnergyColorName => "defect";//"marisa";

    // 卡池的主题色。通常是卡牌框架的颜色。
    public override Color DeckEntryCardColor => new("000A7D");

    public override Color EnergyOutlineColor => new("000A7D");

    // 卡池是否是无色。例如事件、状态等卡池就是无色的。
    public override bool IsColorless => false;

    public override string BigEnergyIconPath => "res://marisamod/images/ui/cardOrb.png";

    public override string TextEnergyIconPath => "res://marisamod/images/ui/energyOrb-lighter.png";
}