using marisamod.Scripts.Characters;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace marisamod.Scripts.Cards;

public class StarlitPotion : AbstractMarisaCard
{
    public StarlitPotion() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }
    
    public override string PortraitPath => "res://marisamod/images/cards/marisamod-test_marisa_card.png";
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cards = Owner.PlayerCombatState!.Hand.Cards.ToArray();
        var enchant = MarisaCharacter.StarlitEnchantment;
        foreach (var card in cards)
        {
            if (enchant.CanEnchant(card))
                CardCmd.Enchant(enchant, card, 1);
        }

        await Cmd.Wait(0.25f);
    }
}