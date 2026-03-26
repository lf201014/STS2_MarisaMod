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
    public class MasterSpark : AbstractAmplifiedCard
    {
        public MasterSpark() : base(1, 1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
        {
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Concat([
            new CalculationBaseVar(8m),
            new ExtraDamageVar(7m),
            new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) => card is AbstractAmplifiedCard { IsAmplified: true } ? 1 : 0)
            ]);

        protected override HashSet<CardTag> CanonicalTags => [MarisaCardTags.Spark];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target);
            await DamageCmd.Attack(DynamicVars.CalculatedDamage).FromCard(this).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash").BeforeDamage(async delegate
            {
                NSweepingBeamVfx nSweepingBeamVfx = NSweepingBeamVfx.Create(base.Owner.Creature, [cardPlay.Target]);
                if (nSweepingBeamVfx != null)
                {
                    NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(nSweepingBeamVfx);
                    await Cmd.Wait(0.5f);
                }
            }).Execute(choiceContext);
            ;
        }

        protected override void OnUpgrade()
        {
            DynamicVars.CalculationBase.UpgradeValueBy(3m);
            DynamicVars.ExtraDamage.UpgradeValueBy(2m);
        }
    }
}