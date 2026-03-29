using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace marisamod.Scripts.Cards
{
    public class OpenUniverse : AbstractMarisaCard
    {
        public OpenUniverse() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
        {
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DynamicVar("Add",5),
            new DynamicVar("Draw",2)
        ];

        protected override void OnUpgrade()
        {
            DynamicVars["Draw"].UpgradeValueBy(1);
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            IEnumerable<CardModel> forCombat =
            CardFactory.GetForCombat(Owner, Owner.Character.CardPool.GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint), DynamicVars["Add"].IntValue, Owner.RunState.Rng.CombatCardGeneration);
            foreach (CardModel item in forCombat)
            {
                CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(item, PileType.Draw, addedByPlayer: true, CardPilePosition.Random));
            }
            var draw = await CardPileCmd.Draw(choiceContext, DynamicVars["Draw"].IntValue, Owner);
            if (draw.Any())
            {
                foreach (var card in draw)
                {
                    CardCmd.Upgrade(card);
                }
            }
        }
    }
}