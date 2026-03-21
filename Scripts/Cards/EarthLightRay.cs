using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace marisamod.Scripts.Cards;

public class EarthLightRay : AbstractAmplifiedCard
{
    public EarthLightRay() : base(0, 1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    public override bool CanBeGeneratedInCombat => false;

    protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Concat([
        new HealVar(4m)
    ]);

    public override IEnumerable<CardKeyword> CanonicalKeywords => base.CanonicalKeywords.Concat([
        CardKeyword.Exhaust
    ]);

    protected override void OnUpgrade()
    {
        DynamicVars.Heal.UpgradeValueBy(2);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
        if (IsAmplified && Owner.PlayerCombatState!.DiscardPile.Cards.Count > 0)
        {
            if (IsUpgraded)
            {
                var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
                var pile = PileType.Discard.GetPile(Owner);
                var cardModel = (await CardSelectCmd.FromSimpleGrid(choiceContext, pile.Cards, base.Owner, prefs)).FirstOrDefault();
                if (cardModel != null)
                {
                    await CardPileCmd.Add(cardModel, PileType.Hand);
                }
            }
            else
            {
                var cardModel = Owner.RunState.Rng.CombatCardSelection.NextItem(PileType.Discard.GetPile(Owner).Cards);
                if (cardModel != null)
                    await CardPileCmd.Add(cardModel, PileType.Hand);
            }
        }
    }
}