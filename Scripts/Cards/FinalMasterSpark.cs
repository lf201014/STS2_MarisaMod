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

namespace marisamod.Scripts.Cards;

public class FinalMasterSpark : AbstractAmplifiedCard
{
    public FinalMasterSpark() : base(1, 1, CardType.Attack, CardRarity.Ancient, TargetType.AllEnemies) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Concat([
        new CalculationBaseVar(16m),
        new ExtraDamageVar(14m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) => card is AbstractAmplifiedCard { IsAmplified: true } ? 1 : 0)
    ]);

    protected override HashSet<CardTag> CanonicalTags => [MarisaCardTags.Spark];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.CalculatedDamage).FromCard(this).TargetingAllOpponents(CombatState!)
            .WithHitFx("vfx/vfx_attack_slash", null, "blunt_attack.mp3")
            .BeforeDamage(async delegate
            {
                List<Creature> enemies = CombatState!.Enemies.Where(e => e.IsAlive).ToList();
                var nHyperbeamVfx = NHyperbeamVfx.Create(Owner.Creature, enemies.Last());
                if (nHyperbeamVfx != null)
                {
                    NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(nHyperbeamVfx);
                    await Cmd.Wait(0.5f);
                }
                foreach (Creature item in enemies)
                {
                    var nHyperbeamImpactVfx = NHyperbeamImpactVfx.Create(Owner.Creature, item);
                    if (nHyperbeamImpactVfx != null)
                    {
                        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(nHyperbeamImpactVfx);
                    }
                }
            })
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(6m);
        DynamicVars.ExtraDamage.UpgradeValueBy(4m);
    }
}