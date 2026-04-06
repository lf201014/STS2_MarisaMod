using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace marisamod.Scripts.Cards
{
    public class ManaRampage : AbstractMarisaCard
    {
        public ManaRampage() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
        {
        }

        protected override bool HasEnergyCostX => true;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            var cards = CardFactory.GetForCombat(Owner, from c in Owner.Character.CardPool.GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
                where c.Type == CardType.Attack
                select c, ResolveEnergyXValue(), Owner.RunState.Rng.CombatCardGeneration).ToArray();

            foreach (var item in cards)
            {
                if (IsUpgraded)
                    CardCmd.Upgrade(item);
                await CardCmd.AutoPlay(choiceContext, item, null);
                await CardCmd.Exhaust(choiceContext, item, false, true);
                //await CardPileCmd.RemoveFromCombat(item, true);
            }
        }
    }
}