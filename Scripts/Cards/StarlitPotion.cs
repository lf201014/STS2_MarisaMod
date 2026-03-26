using marisamod.Scripts.Characters;
using marisamod.Scripts.Enchantments;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace marisamod.Scripts.Cards;

public class StarlitPotion : AbstractMarisaCard
{
    public StarlitPotion() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    //public override string PortraitPath => "res://marisamod/images/cards/marisamod-test_marisa_card.png";

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override void OnUpgrade()
    {
        //RemoveKeyword(CardKeyword.Exhaust);
        EnergyCost.UpgradeBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cards = Owner.PlayerCombatState!.Hand.Cards.ToArray();
        foreach (var card in cards)
        {
            var enchant = ModelDb.Enchantment<StarlitEnchantment>().ToMutable();
            if (enchant.CanEnchant(card))
                MarisaCharacter.Enchant(enchant, card);
        }

        await Cmd.Wait(0.25f);
    }
}