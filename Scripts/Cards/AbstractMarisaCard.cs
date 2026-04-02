using BaseLib.Abstracts;
using BaseLib.Utils;
using marisamod.Scripts.PatchesNModels;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace marisamod.Scripts.Cards;

[Pool(typeof(MarisaCardPool))]
public abstract class AbstractMarisaCard : CustomCardModel
{
    public override string PortraitPath => $"res://marisamod/images/cards/{Id.Entry.ToLowerInvariant()}.png";

    public AbstractMarisaCard(int energyCost, CardType type, CardRarity rarity, TargetType targetType, bool shouldShowInCardLibrary = true, bool autoAdd = true)
        : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary, autoAdd)
    {
    }

    public Task DoFlash()
    {
        if (NCombatRoom.Instance != null)
        {
            var hand = NCombatRoom.Instance.Ui.Hand;
            if (hand.GetCardHolder(this) is NHandCardHolder holder)
            {
                holder.Flash();
            }
        }

        return Task.CompletedTask;
    }
}