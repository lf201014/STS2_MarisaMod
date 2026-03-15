using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;

public class DarkMatter : CustomCardModel
{
    public DarkMatter() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }
}