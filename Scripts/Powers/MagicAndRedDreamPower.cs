using marisamod.Scripts.Cards;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace marisamod.Scripts.Powers;

public class MagicAndRedDreamPower : AbstractMarisaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private bool _toTrigger = false;

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player)
            return Task.CompletedTask;

        _toTrigger = false;
        if (cardPlay.Card.Type is CardType.Attack && Owner.HasPower<ChargeUpPower>() && Owner.GetPower<ChargeUpPower>()!.CalculateMult() > 1)
        {
            _toTrigger = true;
        }
        else if (cardPlay.Card is AbstractAmplifiedCard { IsAmplified: true })
        {
            _toTrigger = true;
        }

        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (_toTrigger)
        {
            _toTrigger = false;
            await CardPileCmd.Draw(context, Amount, Owner.Player);

            var array = (await CardSelectCmd.FromHand(context, Owner.Player, new CardSelectorPrefs(SelectionScreenPrompt, 1), null, this)).ToArray();
            if (array.Length != 0)
            {
                await CardPileCmd.Add(array, PileType.Draw, CardPilePosition.Top);
            }
        }
    }
}