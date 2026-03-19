using BaseLib.Abstracts;
using BaseLib.Utils;
using marisamod.Scripts.PatchesNModels;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace marisamod.Scripts.Cards.Abstract;

[Pool(typeof(MarisaCardPool))]
public abstract class AbstractMarisaCard : CustomCardModel
{
    public override string PortraitPath => $"res://marisamod/images/cards/{Id.Entry.ToLowerInvariant()}.png";

    public AbstractMarisaCard(int energyCost, CardType type, CardRarity rarity, TargetType targetType, bool shouldShowInCardLibrary = true, bool autoAdd = true)
        : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary, autoAdd)
    {
    }
}