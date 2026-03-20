using marisamod.scripts.Cards.Abstract;
using marisamod.scripts.PatchesNModels;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MarisaMod.scripts.Cards
{
    public class MasterSpark : AbstractAmplifiedCard
    {
        public MasterSpark() : base(1, 1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
        {
        }

        // protected override HashSet<CardTag> CanonicalTags => [MarisaCardKeyWords.Spark];
        // public override IEnumerable<CardKeyword> CanonicalKeywords => base.CanonicalKeywords.Concat([MarisaCardKeyWords.SPARK]);


        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new CalculationBaseVar(8m),
            new ExtraDamageVar(7m),
            new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) => card is AbstractAmplifiedCard { IsAmplified: true } ? 1 : 0),
            new EnergyVar(1)
            ];
        
        protected override HashSet<CardTag> CanonicalTags => [MarisaCardTags.Spark];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target);
            await DamageCmd.Attack(DynamicVars.CalculatedDamage).FromCard(this).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.CalculationBase.UpgradeValueBy(3m);
            DynamicVars.ExtraDamage.UpgradeValueBy(2m);
        }
    }
}