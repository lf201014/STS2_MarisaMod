using marisamod.Scripts.Cards;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace marisamod.Scripts.Powers;

public class EventHorizonPower : AbstractMarisaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private int _triggerCounterForTurn;
    private bool _triggerFlag;

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (_triggerFlag)
        {
            _triggerFlag = false;
            var cards = PileType.Discard.GetPile(Owner.Player).Cards.Where(c => c.Type == CardType.Attack).ToArray();
            if (cards.Length > 0)
            {
                var prefs = new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 1);
                var cardModel =
                    (await CardSelectCmd.FromSimpleGrid(context, cards, Owner.Player, prefs)).FirstOrDefault();
                if (cardModel != null)
                {
                    await CardPileCmd.Add(cardModel, PileType.Hand);
                }
            }
        }
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card is not AbstractAmplifiedCard { IsAmplified: true } || _triggerCounterForTurn > Amount) return Task.CompletedTask;
        _triggerCounterForTurn++;
        _triggerFlag = true;
        return Task.CompletedTask;
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return Task.CompletedTask;
        _triggerCounterForTurn = 0;
        _triggerFlag = false;
        return Task.CompletedTask;
    }
}