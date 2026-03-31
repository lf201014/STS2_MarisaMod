using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Cards
{
    public class Occultation : AbstractMarisaCard
    {
        public Occultation() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
        {
        }

        public override bool GainsBlock => true;

        protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(0m),
        new CalculationExtraVar(1m),
        new CalculatedBlockVar(ValueProp.Move).WithMultiplier((card, _) => CardPile.Get(PileType.Draw,card.Owner)?.Cards.Count??0)
            ];

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.CalculatedBlock.Calculate(cardPlay.Target), DynamicVars.CalculatedBlock.Props, cardPlay);

            foreach (var item in CardPile.Get(PileType.Draw, Owner).Cards.ToArray())
            {
                await CardCmd.Discard(choiceContext, item);
            }
        }
    }
}