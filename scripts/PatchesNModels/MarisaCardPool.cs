using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;

namespace marisamod.Scripts.PatchesNModels;

public class MarisaCardPool : CustomCardPoolModel
{
    private const string FramePathAttack = "res://marisamod/images/ui/bg_attack_MRS.png";
    private const string FramePathPower = "res://marisamod/images/ui/bg_power_MRS.png";
    private const string FramePathSkill = "res://marisamod/images/ui/bg_skill_MRS.png";

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

    // public override float H => 0.65f;
    // public override float S => 0.59f;
    // public override float V => 0.69f;

    public override Texture2D CustomFrame(CustomCardModel card)
    {
        var path = card.Type switch
        {
            CardType.Attack => FramePathAttack,
            CardType.Power => FramePathPower,
            _ => FramePathSkill
        };
        return PreloadManager.Cache.GetTexture2D(path);
    }
}