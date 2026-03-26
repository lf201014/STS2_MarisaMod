using marisamod.Scripts.Characters;
using marisamod.Scripts.Enchantments;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Cards;

public class MagicalR360 : AbstractMarisaCard
{
    public MagicalR360() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
    }

    //no art yet
    //public override string PortraitPath => "res://marisamod/images/cards/marisamod-test_marisa_card.png";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(7, ValueProp.Move)
    ];

    public override bool GainsBlock => true;

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

        var zaEnchantment = ModelDb.Enchantment<StarlitEnchantment>().ToMutable();
        var cardModel =
            (await CardSelectCmd.FromHand(
                choiceContext,
                Owner,
                new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1),
                model => zaEnchantment.CanEnchant(model),
                this)
            ).FirstOrDefault();
        if (cardModel != null)
        {
            MarisaCharacter.Enchant(zaEnchantment, cardModel);
        }
    }
}