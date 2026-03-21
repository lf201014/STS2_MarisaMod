using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Cards
{
    public class MilkyWay : AbstractMarisaCard
    {
        public MilkyWay() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
        {
        }

        public override bool GainsBlock => true;
        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new BlockVar(5m, ValueProp.Move),
        new DynamicVar("Add",1)
            ];

        protected override void OnUpgrade()
        {
            DynamicVars.Block.UpgradeValueBy(2m);
            DynamicVars["Add"].UpgradeValueBy(1);
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

            await CardPileCmd.Draw(choiceContext, 1, Owner);

            foreach (CardModel card in Owner.PlayerCombatState!.Hand.Cards.Where(x => x.Type == CardType.Attack).ToArray())
            {
                //Hook.ModifyDamage(base.Owner.RunState, base.Owner.Creature.CombatState, null, base.Owner.Creature, DynamicVars["Add"].BaseValue, ValueProp.Move, card, ModifyDamageHookType.All, CardPreviewMode.None, out IEnumerable<AbstractModel> _);

                if (card.DynamicVars.ContainsKey("CalculatedDamage"))
                {
                    card.DynamicVars.CalculationBase.UpgradeValueBy(DynamicVars["Add"].BaseValue);
                }
                else if (card.DynamicVars.ContainsKey("Damage"))
                {
                    card.DynamicVars.Damage.UpgradeValueBy(DynamicVars["Add"].BaseValue);
                }
            }
        }
    }
}