using marisamod.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Cards
{
    public class DeepEcologicalBomb : AbstractAmplifiedCard
    {
        public DeepEcologicalBomb() : base(1, 1, CardType.Attack, CardRarity.Uncommon, TargetType.RandomEnemy)
        {
        }

        //public override string PortraitPath => $"res://img/cards/DeepEcoBomb_p.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(7,ValueProp.Move),
            new DynamicVar("Power", 2m),
            new EnergyVar(1)
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            var combatState = CombatState ?? Owner.Creature.CombatState;
            if (combatState == null)
            {
                return;
            }
            int repeat = IsAmplified ? 2 : 1;
            for (int i = 0; i < repeat; i++)
            {
                var target = Owner.RunState.Rng.CombatTargets.NextItem(combatState.HittableEnemies);
                if (target != null)
                {
                    await PowerCmd.Apply<DeepEcologicalBombPower>(target, DynamicVars["Power"].BaseValue, Owner.Creature, this);
                    await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(target)
                        .WithHitFx("vfx/vfx_attack_blunt")
                        .Execute(choiceContext);
                }
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(2);
            DynamicVars["Power"].UpgradeValueBy(1m);
        }
    }
}