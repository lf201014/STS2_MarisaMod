using MarisaMod.scripts.Cards.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MarisaMod.scripts.Cards
{
    public class PulseMagic : AbstractAmplifiedCard
    {
        public PulseMagic() : base(0, 1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
        {
        }

        public override string PortraitPath => $"res://img/cards/pulseMagic_p.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new EnergyVar("GainEnergy", 1),
        new EnergyVar("LoseEnergy", 1)
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await PowerCmd.Apply<EnergyNextTurnPower>(Owner.Creature, DynamicVars.Energy.BaseValue, Owner.Creature, this);
            if (IsAmplified)
            {
                await PowerCmd.Apply<PulseMagicePower>(Owner.Creature, 1m, Owner.Creature, this);
            }
        }

        override protected void OnUpgrade()
        {
            DynamicVars["GainEnergy"].UpgradeValueBy(1);
        }
    }
}