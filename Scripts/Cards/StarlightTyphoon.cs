using marisamod.Scripts.Cards.Colorless;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace marisamod.Scripts.Cards;

public class StarlightTyphoon : AbstractMarisaCard
{
    public StarlightTyphoon() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Spark>(IsUpgraded)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cards2Exhaust = GetCards().ToList();
        var count = cards2Exhaust.Count;
        foreach (CardModel item in cards2Exhaust)
        {
            await CardCmd.Exhaust(choiceContext, item);
        }

        var sparks = await Spark.CreateInHand(Owner, count, CombatState!);
        if (IsUpgraded)
            foreach (var spark in sparks)
            {
                CardCmd.Upgrade(spark);
            }
    }

    private IEnumerable<CardModel> GetCards()
    {
        CardPile pile = PileType.Hand.GetPile(base.Owner);
        return pile.Cards.Where((CardModel c) => c.Type != CardType.Attack);
    }
}