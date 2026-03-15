using MarisaMod.scripts.Cards.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MarisaMod.scripts.Cards
{
    public class SparkStrike : AbstractMarisaModCard
    {
        public SparkStrike() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
        {
        }

        public override string PortraitPath => $"res://img/cards/SimpleSpark_p.png";

        protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];

        protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move)];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(3m);
        }
    }
}