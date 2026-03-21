using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Cards
{
    public class Rob : AbstractAmplifiedCard
    {
        public Rob() : base(1, 1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
        {
        }

        override public IEnumerable<CardKeyword> CanonicalKeywords => base.CanonicalKeywords.Concat([
            CardKeyword.Exhaust
        ]);

        override protected void OnUpgrade()
        {
            DynamicVars.CalculationBase.UpgradeValueBy(3);
            DynamicVars.ExtraDamage.UpgradeValueBy(3);
        }

        override protected IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Concat([
            new CalculationBaseVar(7m),
            new ExtraDamageVar(7m),
            new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) => card is AbstractAmplifiedCard { IsAmplified: true } ? 1 : 0)
        ]);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target);
            var attackCommand = await DamageCmd.Attack(DynamicVars.CalculatedDamage.Calculate(cardPlay.Target)).FromCard(this).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
            var gain = attackCommand.Results.Sum(r => r.UnblockedDamage);
            if (gain > 0)
            {
                await PowerCmd.Apply<RoyaltiesPower>(Owner.Creature, gain, Owner.Creature, this);
            }
        }
    }
}