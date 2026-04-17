using marisamod.Scripts.Characters;
using marisamod.Scripts.Enchantments;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace marisamod.Scripts.Relics;

public class UnstableMagicTool : AbstractMarisaRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        HoverTipFactory.FromEnchantment<StarlitEnchantment>();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3)
    ];

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is CombatRoom)
        {
            Flash();
            var zaEnchantment = ModelDb.Enchantment<StarlitEnchantment>().ToMutable();
            var cards = PileType.Draw.GetPile(Owner).Cards.Where(x => zaEnchantment.CanEnchant(x)).ToList().StableShuffle(Owner.RunState.Rng.CombatCardSelection)
                .Take(DynamicVars.Cards.IntValue)
                .ToList();
            foreach (var card in cards)
            {
                //MarisaCharacter.Enchant(zaEnchantment, card);
                CardCmd.Enchant(zaEnchantment, card, 1);
                zaEnchantment = ModelDb.Enchantment<StarlitEnchantment>().ToMutable();
            }

            CardCmd.Preview(cards);
        }

        return Task.CompletedTask;
    }
}