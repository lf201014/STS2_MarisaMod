using Godot;
using marisamod.Scripts.PatchesNModels;
using marisamod.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace marisamod.Scripts.Cards;

public class CardSwiper : AbstractMarisaCard
{
    public CardSwiper() : base(1, CardType.Power, CardRarity.Rare, TargetType.AllAllies)
    {
    }
    public override bool CanBeGeneratedInCombat => false;
    private static readonly Func<CardModel, bool>[] StealPriorities =
    [
	    c => c.Enchantment is not Imbued && c.Rarity == CardRarity.Uncommon,//无附魔罕见卡
	    delegate(CardModel c) //无附魔稀有普通事件卡
	    {
		    if (c.Enchantment is Imbued) return false;
		    return c.Rarity is CardRarity.Common or CardRarity.Rare or CardRarity.Event;
	    },
	    delegate(CardModel c)//无附魔基础或任务卡
	    {
		    if (c.Enchantment is Imbued) return false;
		    return c.Rarity is CardRarity.Basic or CardRarity.Quest;
	    },
	    c => c.Rarity == CardRarity.Ancient || c.Enchantment is Imbued//远古卡或附魔卡
    ];
    
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    public override IEnumerable<CardKeyword> CanonicalKeywords => base.CanonicalKeywords.Concat([
        //CardKeyword.Exhaust,
        MarisaCardKeyWords.Steal,
    ]);
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var players = CombatState!.GetTeammatesOf(Owner.Creature)
            .Where(c => c is { IsAlive: true, IsPlayer: true } && c != Owner.Creature);
        List<CardModel> stolenCard = [];
        foreach (var player in players)
        {
            var cardToSteal = GetCard(player.Player!);
            if (cardToSteal == null) continue;
            await CardPileCmd.RemoveFromCombat(cardToSteal);
            CardModel copy = GetCardClone(cardToSteal);
            stolenCard.Add(copy);
            if (player.HasPower<SwipedCardPower>())
            {
                var pow = player.GetPower<SwipedCardPower>()!;
                await pow.Steal(cardToSteal);
            }
            else
            {
                var pow = (SwipedCardPower)ModelDb.Power<SwipedCardPower>().ToMutable();
                await pow.Steal(cardToSteal);
                await PowerCmd.Apply(new ThrowingPlayerChoiceContext(), pow, player, 1M, Owner.Creature, this);
            }
        }
        await Cmd.Wait(0.2f);
        await CardPileCmd.Add(stolenCard, PileType.Hand);
        CardCmd.Preview(stolenCard);
    }

    private CardModel? GetCard(Player target)
    {
        // List<CardModel> list = (from c in CardPile.GetCards(target, PileType.Draw, PileType.Discard)
        //     where c.DeckVersion != null
        //     select c).ToList();
        var list = CardPile.GetCards(target, PileType.Draw).ToList();
        if (list.Count == 0) return null;
        IEnumerable<CardModel> cards = list;
        var stealPriorities = StealPriorities;
        foreach (var predicate in stealPriorities)
        {
	        var subset = list.Where(predicate).ToList();
	        if (subset.Count == 0) continue;
	        cards = subset;
	        break;
        }
        //return list.TakeRandom(1, Owner.RunState.Rng.CombatCardSelection).FirstOrDefault();
        return Owner.RunState.Rng.CombatCardSelection.NextItem(cards);
    }
	
    private CardModel GetCardClone(CardModel card)
    {
        CardModel copy = CombatState!.CreateCard(ModelDb.GetById<CardModel>(card.Id),Owner);
        if (card.Enchantment != null && copy.Enchantment == null)
        {
            var zaEnchantment = ModelDb.GetById<EnchantmentModel>(card.Enchantment.Id).ToMutable();
            CardCmd.Enchant(zaEnchantment, copy, card.Enchantment.Amount);
        }
        copy.DynamicVars.Clone(card);
        return copy;
    }
}