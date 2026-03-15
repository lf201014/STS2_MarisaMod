using MarisaMod.scripts.Cards.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MarisaMod.scripts.Cards
{
    public class DarkSpark : AbstractMarisaModCard
    {
        public DarkSpark() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
        {
        }

        public override string PortraitPath => $"res://img/cards/darkSpark_p.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(7m, ValueProp.Move),
            new CardsVar(5)
            ];

        protected override void OnUpgrade()
        {
            DynamicVars.Cards.UpgradeValueBy(3m);
        }

        override protected async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            CardPile drawPile = PileType.Draw.GetPile(Owner);
            int countOfAttacks = 0;
            for (int i = 0; i < DynamicVars.Cards.IntValue; i++)
            {
                await CardPileCmd.ShuffleIfNecessary(choiceContext, Owner);
                CardModel? cardModel = drawPile.Cards.FirstOrDefault();
                if (cardModel != null)
                {
                    if (cardModel.Type == CardType.Attack)
                    {
                        countOfAttacks++;
                    }
                    await CardCmd.Exhaust(choiceContext, cardModel);
                }
            }
            if (countOfAttacks > 0 && CombatState != null)
                await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(countOfAttacks).FromCard(this).TargetingAllOpponents(CombatState)
                    .WithHitFx("vfx/vfx_attack_slash")
                    .Execute(choiceContext);
        }
    }
}