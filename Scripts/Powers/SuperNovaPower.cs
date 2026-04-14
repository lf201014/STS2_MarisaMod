using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace marisamod.Scripts.Powers
{
    public class SuperNovaPower : AbstractMarisaPower
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Counter;

        // public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        // {
        //     if (side != CombatSide.Player)
        //     {
        //         return;
        //     }
        //     if (Owner.Player != null)
        //         foreach (CardModel item in CardPile.GetCards(Owner.Player, PileType.Hand).ToList())
        //         {
        //             await CardCmd.Exhaust(choiceContext, item);
        //         }
        // }

        public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side == CombatSide.Player)
                foreach (var item in await CardSelectCmd.FromHand(choiceContext, Owner.Player!,
                             new CardSelectorPrefs(SelectionScreenPrompt, 0, 2), null, this))
                {
                    await CardCmd.Discard(choiceContext, item);
                }
        }

        public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
        {
            if (card is Burn)
            {
                await PowerCmd.Apply<StrengthPower>(Owner, Amount, Owner, null);
            }
        }

        public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            foreach (var allCard in Owner.Player!.PlayerCombatState!.AllCards)
            {
                await ApplyEthereal(allCard);
            }
        }

        public override async Task AfterCardEnteredCombat(CardModel card)
        {
            if (card.Owner == Owner.Player)
            {
                await ApplyEthereal(card);
            }
        }

        private static Task ApplyEthereal(CardModel card)
        {
            if (!card.Keywords.Contains(CardKeyword.Ethereal))
            {
                CardCmd.ApplyKeyword(card, CardKeyword.Ethereal);
            }

            return Task.CompletedTask;
        }
    }
}