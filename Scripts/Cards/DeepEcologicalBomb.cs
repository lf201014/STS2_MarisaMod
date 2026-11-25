using marisamod.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Cards
{
    public class DeepEcologicalBomb : AbstractAmplifiedCard
    {
        public DeepEcologicalBomb() : base(1, 1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
        {
        }

        //public override string PortraitPath => $"res://img/cards/DeepEcoBomb_p.png";

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(7, ValueProp.Move),
            new DynamicVar("Power", 2m),
            new EnergyVar(1)
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target);
            await base.OnPlay(choiceContext, cardPlay);
            var combatState = CombatState ?? Owner.Creature.CombatState;
            if (combatState == null)
            {
                return;
            }

            //var damage = DynamicVars.Damage.BaseValue;

            var repeat = AmplifiedInPlay ? 2 : 1;
            //for (var i = 0; i < repeat; i++)
            {
                //var target = Owner.RunState.Rng.CombatTargets.NextItem(combatState.HittableEnemies);
                //if (target == null) continue;
                //await PowerCmd.Apply<DeepEcologicalBombPower>(choiceContext, target, DynamicVars["Power"].BaseValue, Owner.Creature, this);
                var dmgCmd = await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this,cardPlay).WithHitCount(repeat)
                    .Targeting(cardPlay.Target)
                    .WithHitFx("vfx/vfx_attack_blunt")
                    .Execute(choiceContext);

                var tars = dmgCmd.Results.Select(list => list.Select(x => x.Receiver)).SelectMany(items => items).ToArray();
                foreach (var creature in tars)
                {
                    if (creature.IsAlive)
                    {
                        await PowerCmd.Apply<DeepEcologicalBombPower>(choiceContext, creature, DynamicVars["Power"].BaseValue, Owner.Creature, this);
                    }
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