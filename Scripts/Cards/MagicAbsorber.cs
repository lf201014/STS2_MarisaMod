using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Cards
{
    public class MagicAbsorber : AbstractMarisaCard
    {
        public MagicAbsorber() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
        {
        }

        public override IEnumerable<CardKeyword> CanonicalKeywords => base.CanonicalKeywords.Concat([
            CardKeyword.Exhaust
        ]);

        public override bool GainsBlock => true;

        protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Concat([
            new BlockVar(10m, ValueProp.Move)//,
            //new BlockVar("ExtraBlock",3,ValueProp.Move)
            ]);

        protected override void OnUpgrade()
        {
            DynamicVars.Block.UpgradeValueBy(4m);
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
            var pows = Owner.Creature.Powers.Where(p => p.Type == PowerType.Debuff).TakeRandom(1, Owner.RunState.Rng.CombatCardSelection).ToArray();
            if (pows.Length > 0)
                await PowerCmd.Remove(pows[0]);

            // if (Owner.PlayerCombatState != null)
            // {
            //     var cards = Owner.PlayerCombatState.Hand.Cards.Where(c => c.Type is CardType.Status).ToArray();
            //     if (cards.Length > 0)
            //         if (IsAmplified)
            //         {
            //             foreach (var item in cards)
            //             {
            //                 await CardCmd.Exhaust(choiceContext, item);
            //                 await CreatureCmd.GainBlock(Owner.Creature, DynamicVars["ExtraBlock"].BaseValue, ValueProp.Move, cardPlay);
            //             }
            //         }
            //         else
            //         {
            //             var cardModel = (await CardSelectCmd.FromHand(
            //                 choiceContext,
            //                 Owner,
            //                 new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1),
            //                 x => x.Type is CardType.Status,
            //                 this)).FirstOrDefault();
            //             if (cardModel != null)
            //             {
            //                 await CardCmd.Exhaust(choiceContext, cardModel);
            //             }
            //         }
            // }
        }
    }
}