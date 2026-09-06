using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Cards
{
    public class NonDirectionalLaser : AbstractMarisaCard
    {
        public NonDirectionalLaser() : base(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
        {
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(7m, ValueProp.Move)];

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(2m);
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            var repeat = 1;
            if (HasGeneratedCard)
                repeat = 2;
            var damage = DynamicVars.Damage.BaseValue;
            await DamageCmd.Attack(damage).FromCard(this,cardPlay).TargetingAllOpponents(CombatState!).WithHitCount(repeat)
                .WithHitFx("vfx/vfx_attack_blunt", null, "heavy_attack.mp3")
                .Execute(choiceContext);
            // await DamageCmd.Attack(damage).FromCard(this,cardPlay)
            //     .TargetingRandomOpponents(CombatState!)
            //     .WithHitFx("vfx/vfx_attack_slash")
            //     .Execute(choiceContext);
        }

        protected override bool ShouldGlowGoldInternal => HasGeneratedCard;

        private bool HasGeneratedCard => CombatManager.Instance.History.Entries.OfType<CardGeneratedEntry>().Any(e => e.Creator == Owner && e.HappenedThisTurn(CombatState));
    }
}