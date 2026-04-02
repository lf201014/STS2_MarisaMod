using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace marisamod.Scripts.Cards;

public class StarDustReverie : AbstractMarisaCard
{
    public StarDustReverie() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var hand = Owner.PlayerCombatState!.Hand.Cards.ToArray();
        var num = hand.Length;
        foreach (var card in hand)
        {
            await CardPileCmd.Add(card, PileType.Draw, CardPilePosition.Random);
        }
        
        var cards =
            CardFactory.GetForCombat(Owner, Owner.Character.CardPool.GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
                , num, Owner.RunState.Rng.CombatCardGeneration);
        foreach (var cardModel in cards)
        {
            if (IsUpgraded)
                CardCmd.Upgrade(cardModel);
            await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, addedByPlayer: true);
        }
    }
}