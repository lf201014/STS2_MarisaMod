using marisamod.Scripts.Powers;
using marisamod.Scripts.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace marisamod.Scripts.Cards;

public class PropBag : AbstractMarisaCard
{
    public PropBag() : base(0, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    private static readonly List<RelicModel?> PoolUncommon =
    [
        ModelDb.Relic<Orichalcum>(),
        ModelDb.Relic<Vambrace>(),
        ModelDb.Relic<RippleBasin>(),
        ModelDb.Relic<GremlinHorn>(),
        ModelDb.Relic<PenNib>(),
        ModelDb.Relic<JossPaper>(),
        ModelDb.Relic<OrnamentalFan>(),
        ModelDb.Relic<LetterOpener>(),
        ModelDb.Relic<ReptileTrinket>(),
        ModelDb.Relic<Nunchaku>(),
        ModelDb.Relic<MercuryHourglass>(),
        ModelDb.Relic<Kusarigama>(),
        ModelDb.Relic<TuningFork>(),
        ModelDb.Relic<MiniatureCannon>(),
        ModelDb.Relic<ParryingShield>(),
        ModelDb.Relic<AmplifyWand>()
    ];

    private static readonly List<RelicModel?> PoolRare =
    [
        ModelDb.Relic<IceCream>(),
        ModelDb.Relic<UnceasingTop>(),
        ModelDb.Relic<RainbowRing>(),
        ModelDb.Relic<CloakClasp>(),
        ModelDb.Relic<MummifiedHand>(),
        ModelDb.Relic<IntimidatingHelmet>(),
        ModelDb.Relic<Shuriken>(),
        ModelDb.Relic<Kunai>(),
        ModelDb.Relic<GamePiece>(),
        ModelDb.Relic<SturdyClamp>(),
        ModelDb.Relic<BeatingRemnant>(),
        ModelDb.Relic<Pocketwatch>(),
        ModelDb.Relic<ArtOfWar>(),
        ModelDb.Relic<RazorTooth>(),
        ModelDb.Relic<TungstenRod>(),
        ModelDb.Relic<MagicBroom>()
    ];

    // public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        //1-9: uncommon; 0: rare
        var odd = Owner.RunState.Rng.CombatCardSelection.NextInt(10);
        var pick = odd == 0
            ? PoolRare.TakeRandom(1, Owner.RunState.Rng.CombatCardSelection).FirstOrDefault()!.ToMutable()
            : PoolUncommon.TakeRandom(1, Owner.RunState.Rng.CombatCardSelection).FirstOrDefault()!.ToMutable();

        var relic = await RelicCmd.Obtain(pick, Owner);

        PropBagPower? pow;
        if (Owner.Creature.HasPower<PropBagPower>())
        {
            pow = Owner.Creature.GetPower<PropBagPower>();
        }
        else
        {
            pow = await PowerCmd.Apply<PropBagPower>(Owner.Creature, 1, Owner.Creature, this);
            pow?.ClearRelicList();
        }

        pow?.AddRelicToList(relic);

        Log.Info($"PropBag.OnPlay: odd: {odd},pick: {pick}, relic: {relic}, pow: {pow}");
    }
}