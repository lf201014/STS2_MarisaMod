using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace marisamod.Scripts.Potions;

public class TestPotion : PotionModel
{
    // 稀有度
    public override PotionRarity Rarity => PotionRarity.Common;

    // 使用方式，CombatOnly表示只能在战斗中使用。
    public override PotionUsage Usage => PotionUsage.CombatOnly;

    // 目标类型
    public override TargetType TargetType => TargetType.Self;

    // 定义数量
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(3)];

    // tooltip显示灵魂
    public override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Soul>()];

    //public string ImagePath => "Test/Assets/Potions/TestPotion.png";

    // 打出时的效果逻辑，这里是创造3张灵魂到手牌中。
    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        // 这里的DynamicVars.Cards.IntValue就是我们在CanonicalVars中定义的CardsVar的数值，也就是3。
        await Soul.CreateInHand(Owner, DynamicVars.Cards.IntValue, Owner.Creature.CombatState);
    }
}
