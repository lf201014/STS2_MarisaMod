using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Cards;

public class DarkMatter : AbstractMarisaCard
{
    public DarkMatter() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5m, ValueProp.Move),
        new CardsVar(1)
    ];

    //public override bool GainsBlock => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => base.CanonicalKeywords.Concat([
        CardKeyword.Ethereal
    ]);

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
    }

    protected override bool IsPlayable =>
        !CombatManager.Instance.History.CardPlaysFinished.Any(e => e.HappenedThisTurn(CombatState) && e.CardPlay.Card is DarkMatter && e.CardPlay.Card.Owner == Owner);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        List<CardModel> cards =
        [
            CreateClone(),
            CreateClone()
        ];
        if (IsUpgraded)
        {
            foreach (var card in cards)
            {
                CardCmd.Upgrade(card);
            }
        }

        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Draw, true, CardPilePosition.Random));
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card != this)
        {
            return;
        }

        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, null);
    }
}