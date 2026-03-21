using marisamod.Scripts.Cards.Colorless;
using marisamod.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace marisamod.Scripts.Cards
{
    public class MaximisePower : AbstractMarisaCard
    {
        public MaximisePower() : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
        {
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new EnergyVar(1)
        ];

        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Exhaustion>()];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            var pow = Owner.Creature.GetPower<ChargeUpPower>();
            if (pow != null)
            {
                var gain = pow.Amount;
                await PowerCmd.Remove(pow);
                await PlayerCmd.GainEnergy(gain, Owner);
            }
            await PowerCmd.Apply<MaximisePowerPower>(Owner.Creature, 1m, Owner.Creature, this);
        }
    }
}