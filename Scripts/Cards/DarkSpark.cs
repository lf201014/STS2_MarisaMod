using marisamod.Scripts.PatchesNModels;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Cards
{
    public class DarkSpark : AbstractMarisaCard
    {
        public DarkSpark() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
        {
        }

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new CalculationBaseVar(14m),
            new ExtraDamageVar(2m),
            new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) => PileType.Exhaust.GetPile(card.Owner).Cards.Count)
        ];

        protected override HashSet<CardTag> CanonicalTags => [MarisaCardTags.Spark];

        protected override void OnUpgrade()
        {
            DynamicVars.CalculationBase.UpgradeValueBy(4m);
            DynamicVars.ExtraDamage.UpgradeValueBy(1m);
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await DamageCmd.Attack(DynamicVars.CalculatedDamage).FromCard(this).TargetingAllOpponents(CombatState)
                .WithHitFx("vfx/vfx_attack_slash", null, "blunt_attack.mp3")
            .BeforeDamage(async delegate
            {
                List<Creature> enemies = base.CombatState.Enemies.Where((Creature e) => e.IsAlive).ToList();
                var nHyperbeamVfx = NHyperbeamVfx.Create(base.Owner.Creature, enemies.Last());
                if (nHyperbeamVfx != null)
                {
                    NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(nHyperbeamVfx);
                    await Cmd.Wait(0.5f);
                }
                foreach (Creature item in enemies)
                {
                    var nHyperbeamImpactVfx = NHyperbeamImpactVfx.Create(base.Owner.Creature, item);
                    if (nHyperbeamImpactVfx != null)
                    {
                        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(nHyperbeamImpactVfx);
                    }
                }
            })
                .Execute(choiceContext);
        }
    }
}