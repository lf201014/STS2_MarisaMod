using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace marisamod.Scripts.Relics
{
    public class HandmadeGrimoire : AbstractMarisaRelic
    {
        public override RelicRarity Rarity => RelicRarity.Uncommon;

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new CardsVar(1),
            new EnergyVar(1),
            new DynamicVar("Div",15)
        ];

        private bool _isActivated;

        public override Task BeforeCombatStart()
        {
            _isActivated = false;
            return Task.CompletedTask;
        }


        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player == Owner && !_isActivated)
            {
                var cnt = Owner.Deck.Cards.Count / DynamicVars["Div"].IntValue;
                if (cnt > 0)
                {
                    await PlayerCmd.GainEnergy(cnt, Owner);
                    await CardPileCmd.Draw(choiceContext, cnt, Owner);
                }
                _isActivated = true;
            }
        }

        // public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
        // {
        //     if (side == Owner.Creature.Side && !_isActivated)
        //     {
        //         var cnt = Owner.Deck.Cards.Count / DynamicVars["Div"].IntValue;
        //         if (cnt > 0)
        //         {
        //             await PlayerCmd.GainEnergy(cnt, Owner);
        //             await CardPileCmd.Draw(choiceContext, cnt, Owner);
        //         }
        //         _isActivated = true;
        //     }
        // }
    }
}