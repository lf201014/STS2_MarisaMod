using marisamod.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace marisamod.Scripts.Relics;

public class BewitchedHakkero : AbstractMarisaRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Power", 1m),
        new DynamicVar("PowerAmp", 2)
    ];


    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner)
        {
            return;
        }
        var cnt = DynamicVars["Power"].IntValue;
        if (cardPlay.Card.Type == CardType.Attack) // && !MehModConfig.NerfHakkero)
            cnt = DynamicVars["PowerAmp"].IntValue;
        await PowerCmd.Apply<ChargeUpPower>(Owner.Creature, cnt, Owner.Creature, null);
        //return base.AfterCardPlayed(context, cardPlay);
    }
}