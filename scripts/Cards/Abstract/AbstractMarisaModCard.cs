using BaseLib.Abstracts;
using BaseLib.Utils;
using MarisaMod.Scripts.PatchesNModels;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace MarisaMod.scripts.Cards.Abstract
{
    [Pool(typeof(MarisaCardPool))]
    public class AbstractMarisaModCard(int baseCost, CardType type, CardRarity rarity, TargetType target, bool showInCardLibrary = true, bool autoAdd = true)
        : CustomCardModel(baseCost, type, rarity, target, showInCardLibrary, autoAdd)
    {
        public override string PortraitPath => $"res://MarisaMod/img/cards/{Id.Entry.ToLowerInvariant()}.png";
    }
}