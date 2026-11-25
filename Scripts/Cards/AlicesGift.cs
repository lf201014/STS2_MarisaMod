using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Cards
{
    public class AlicesGift : AbstractAmplifiedCard
    {
        public AlicesGift() : base(0, 2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
        {
        }

        public override IEnumerable<CardKeyword> CanonicalKeywords => base.CanonicalKeywords.Concat([
            CardKeyword.Retain
            //CardKeyword.Exhaust
        ]);

        // protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Concat([
        //     new CalculationBaseVar(5m),
        //     new ExtraDamageVar(5m),
        //     new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) => card is AbstractAmplifiedCard { IsAmplified: true } ? 2 : 0)
        // ]);

        protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Concat([
            new DamageVar(5, ValueProp.Move),
            new RepeatVar(3)
        ]);


        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await base.OnPlay(choiceContext, cardPlay);
            ArgumentNullException.ThrowIfNull(cardPlay.Target);
            var repeat = AmplifiedInPlay ? DynamicVars.Repeat.IntValue : 1;
            //await DamageCmd.Attack(DynamicVars.CalculatedDamage).FromCard(this,cardPlay).Targeting(cardPlay.Target)
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(repeat).FromCard(this,cardPlay).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }

        protected override void OnUpgrade()
        {
            // DynamicVars.CalculationBase.UpgradeValueBy(2m);
            // DynamicVars.ExtraDamage.UpgradeValueBy(2m);
            DynamicVars.Damage.UpgradeValueBy(2m);
        }
    }
}