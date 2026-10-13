using marisamod.Scripts.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace marisamod.Scripts.Cards;

public class HeatDeathOfUniverse : AbstractMarisaCard
{
    public HeatDeathOfUniverse() : base(1, CardType.Skill, CardRarity.Rare, TargetType.AllAllies)
    {
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var sum = 0;
        var playersAvail = new List<Player>();
        var enumerable = CombatState!.GetTeammatesOf(Owner.Creature).Where(c => c is { IsAlive: true, IsPlayer: true });
        foreach (var creature in enumerable)
        {
            if (!playersAvail.Contains(creature.Player!))
                playersAvail.Add(creature.Player!);
        }

        foreach (var player in playersAvail)
        {
            foreach (var item in await CardSelectCmd.FromHand(choiceContext, player, new CardSelectorPrefs(SelectionScreenPrompt, 0, 99), null, this))
            {
                await CardCmd.Exhaust(choiceContext, item);
                sum += 1;
            }
        }

        await PowerCmd.Apply<ChargeUpPower>(choiceContext, Owner.Creature, DynamicVars["Power"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}