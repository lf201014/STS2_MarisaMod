using marisamod.Scripts.Characters;
using marisamod.Scripts.Enchantments;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace marisamod.Scripts.Cards;

public class MagicChant : AbstractMarisaCard
{
    public MagicChant() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
        List<CardModel> cardsIn = (from c in PileType.Draw.GetPile(Owner).Cards
            orderby c.Rarity, c.Id
            select c).ToList();
        var cardModel = (await CardSelectCmd.FromSimpleGrid(choiceContext, cardsIn, Owner, prefs)).FirstOrDefault();
        var enchant = ModelDb.Enchantment<StarlitEnchantment>().ToMutable();
        if (cardModel != null)
        {
            if (enchant.CanEnchant(cardModel))
                MarisaCharacter.Enchant(enchant, cardModel);
            await CardPileCmd.Add(cardModel, PileType.Hand);
        }
    }
}