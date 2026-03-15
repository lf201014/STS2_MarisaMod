using MarisaMod.scripts.Cards.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace MarisaMod.scripts.Cards
{
    public class AsteroidBelt : AbstractAmplifiedCard
    {
        public AsteroidBelt() : base(1, 1, CardType.Skill, CardRarity.Common, TargetType.Self)
        {
        }
        public override string PortraitPath => $"res://img/cards/Asteroid_p.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
                new BlockVar(8m,ValueProp.Move),
                new EnergyVar(1)
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            decimal amount = await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
            if (IsAmplified)
                await PowerCmd.Apply<BlockNextTurnPower>(Owner.Creature, amount, Owner.Creature, this);
        }

        override protected void OnUpgrade()
        {
            DynamicVars.Block.UpgradeValueBy(3m);
        }
    }
}