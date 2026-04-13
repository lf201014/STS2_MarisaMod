using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace marisamod.Scripts.Cards;

public class BigCrunch : AbstractMarisaCard
{
    public BigCrunch() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Div", 5m),
        new EnergyVar(1),
        new CalculationBaseVar(0m),
        new CalculationExtraVar(1m),
        new CalculatedVar("CalculatedDraw").WithMultiplier((card, _)
            => Mathf.FloorToInt(
                (Mathf.Floor(PileType.Draw.GetPile(card.Owner).Cards.Count / 2f) + Mathf.Floor(PileType.Discard.GetPile(card.Owner).Cards.Count / 2f))
                / card.DynamicVars["Div"].IntValue))
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var draw = (int)((CalculatedVar)DynamicVars["CalculatedDraw"]).Calculate(cardPlay.Target);
        await DoExhaust(choiceContext, PileType.Draw.GetPile(Owner));
        await DoExhaust(choiceContext, PileType.Discard.GetPile(Owner));
        if (draw > 0)
        {
            await CardPileCmd.Draw(choiceContext, draw, Owner);
            await PlayerCmd.GainEnergy(draw, Owner);
        }
    }

    private async Task DoExhaust(PlayerChoiceContext choiceContext, CardPile pile)
    {
        var res = pile.Cards.Count / 2;
        List<CardModel> toExhaust = [];
        for (var i = 0; i < res; i++)
        {
            //await CardPileCmd.ShuffleIfNecessary(choiceContext, base.Owner);
            var cardModel = Owner.RunState.Rng.CombatCardSelection.NextItem(pile.Cards);
            if (cardModel != null)
            {
                toExhaust.Add(cardModel);
            }
        }
        if (toExhaust.Count > 0)
        {
            CardCmd.Preview(toExhaust);
            foreach (var cardModel in toExhaust)
            {
                await CardCmd.Exhaust(choiceContext, cardModel, skipVisuals: true);
                
            }
            pile.InvokeContentsChanged();
            pile.InvokeCardRemoveFinished();
            pile.InvokeCardAddFinished();
            PileType.Exhaust.GetPile(Owner).InvokeContentsChanged();
            PileType.Exhaust.GetPile(Owner).InvokeCardRemoveFinished();
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Div"].UpgradeValueBy(-1);
    }
}