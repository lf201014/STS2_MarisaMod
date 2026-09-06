using BaseLib.Abstracts;
using marisamod.Scenes.Vfx.HitVfx;
using marisamod.Scripts.PatchesNModels;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Cards
{
    public class MasterSpark : AbstractAmplifiedCard, ITranscendenceCard
    {
        public MasterSpark() : base(1, 1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
        {
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Concat([
            // new CalculationBaseVar(8m),
            // new ExtraDamageVar(7m),
            // new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) => card is AbstractAmplifiedCard { IsAmplified: true } ? 1 : 0)
            new DamageVar(8, ValueProp.Move),
            new DamageVar("DamageAmplified", 15, ValueProp.Move)
        ]);

        protected override HashSet<CardTag> CanonicalTags => [MarisaCardTags.Spark];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await base.OnPlay(choiceContext, cardPlay);
            ArgumentNullException.ThrowIfNull(cardPlay.Target);
            var damage = !AmplifiedInPlay ? DynamicVars.Damage.BaseValue : DynamicVars["DamageAmplified"].BaseValue;
            await DamageCmd.Attack(damage).FromCard(this,cardPlay).Targeting(cardPlay.Target)
                .WithHitVfxNode((Creature t) => SparkHitVfx.Create(NCombatRoom.Instance?.GetCreatureNode(t)!,"BurstSpark"))
                .BeforeDamage(async delegate
                {
                    NSweepingBeamVfx nSweepingBeamVfx = NSweepingBeamVfx.Create(Owner.Creature, [cardPlay.Target])!;
                    NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(nSweepingBeamVfx);
                    await Cmd.Wait(0.5f);
                }).Execute(choiceContext);
        }

        protected override void OnUpgrade()
        {
            // DynamicVars.CalculationBase.UpgradeValueBy(3m);
            // DynamicVars.ExtraDamage.UpgradeValueBy(2m);
            DynamicVars.Damage.UpgradeValueBy(3);
            DynamicVars["DamageAmplified"].UpgradeValueBy(5);
        }

        public CardModel GetTranscendenceTransformedCard()
        {
            return ModelDb.Card<FinalMasterSpark>();
        }
    }
}