using BaseLib.Abstracts;
using BaseLib.Utils;
using MarisaMod.Scripts.PatchesNModels;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace MarisaMod.scripts.Cards.Abstract
{
    [Pool(typeof(MarisaCardPool))]
    public class AbstractMarisaModCard : CustomCardModel
    {
        //TODO
        //public override string PortraitPath => $"res://img/cards/{Id.Entry.ToLowerInvariant()}.png";

        public AbstractMarisaModCard(int baseCost, CardType type, CardRarity rarity, TargetType target, bool showInCardLibrary = true, bool autoAdd = true) : base(baseCost, type, rarity, target, showInCardLibrary, autoAdd)
        {
        }
    }
}