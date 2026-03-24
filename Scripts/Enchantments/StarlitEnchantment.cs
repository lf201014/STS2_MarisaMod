using marisamod.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace marisamod.Scripts.Enchantments;

public class StarlitEnchantment : AbstractMarisaEnchantment
{
    public override bool CanEnchant(CardModel card)
    {
        return card.Enchantment == null && !card.Keywords.Contains(CardKeyword.Unplayable);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card == Card && cardPlay.Resources.EnergySpent > 0)
        {
            await PowerCmd.Apply<StarlitPower>(Card.Owner.Creature, cardPlay.Resources.EnergySpent, Card.Owner.Creature, Card);
        }
    }
}