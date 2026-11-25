using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Powers;
public class SwipedCardPower : AbstractMarisaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.None;
    private List<CardModel> _stolenCards =[];  
    protected override void AfterCloned()
    {
        _stolenCards = [];
    }
    public override Task AfterCombatEnd(CombatRoom room)
    {
        var player = Owner.Player;
        if (player == null || _stolenCards.Count == 0) return Task.CompletedTask;

        var runState = CombatState.RunState;
        foreach (var card in _stolenCards)
        {
            if (card.DeckVersion == null) continue;
            runState.AddCard(card.DeckVersion, player);
            SpecialCardReward specialCardReward = new SpecialCardReward(card.DeckVersion, player);
            specialCardReward.SetCustomDescriptionEncounterSource(ModelDb.Encounter<ThievingHopperWeak>().Id);
            ((CombatRoom)runState.CurrentRoom!).AddExtraReward(player, specialCardReward);
        }
        _stolenCards.Clear();
        return Task.CompletedTask;
    }

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature,CardModel? source,
        bool wasRemovalPrevented, float deathAnimLength)
    {
        var player = Owner.Player;
        if (player == null || _stolenCards == null || _stolenCards.Count == 0) return;

        IRunState runState = CombatState.RunState;
        foreach (var card in _stolenCards)
        {
            if (card.DeckVersion == null) continue;
            runState.AddCard(card.DeckVersion, player);
            SpecialCardReward specialCardReward = new SpecialCardReward(card.DeckVersion, player);
            specialCardReward.SetCustomDescriptionEncounterSource(ModelDb.Encounter<ThievingHopperWeak>().Id);
            ((CombatRoom)runState.CurrentRoom!).AddExtraReward(player, specialCardReward);
        }
        _stolenCards.Clear();
    }

    public async Task Steal(CardModel card) 
    {
        var player = card.Owner.Creature.Player;
        if (player == null) return;
        if (card.DeckVersion != null)
        {
            await CardPileCmd.RemoveFromDeck(card.DeckVersion, showPreview: false);
            _stolenCards.Add(card);
        }
    }
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => _stolenCards.Select(c => HoverTipFactory.FromCard(c)).ToArray();
}