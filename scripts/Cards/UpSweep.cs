using MarisaMod.scripts.Cards.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MarisaMod.scripts.Cards
{
    public class UpSweep : AbstractMarisaModCard
    {
        public UpSweep() : base(0, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy, true, true)
        {
        }

        public override string PortraitPath => $"res://img/cards/UpSweep_p.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
          new DamageVar(4m, ValueProp.Move),
          new DynamicVar("Power", 1m)
          ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
            await PowerCmd.Apply<ChargeUpPower>(base.Owner.Creature, DynamicVars.Strength.BaseValue, base.Owner.Creature, this);
        }

        override protected void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(1m);
            DynamicVars["Power"].UpgradeValueBy(1m);
        }
    }
}