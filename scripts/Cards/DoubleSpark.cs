using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MarisaMod.Scripts.Cards.Colorless;
using MegaCrit.Sts2.Core.HoverTips;
using MarisaMod.scripts.Cards.Abstract;

namespace MarisaMod.scripts.Cards
{
    public class DoubleSpark : AbstractMarisaModCard
    {
        public DoubleSpark() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
        {
        }

        public override string PortraitPath => $"res://img/cards/DoubleSpark_p.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(6m, ValueProp.Move)
        ];
        
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Spark>(IsUpgraded)];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
            if (CombatState != null)
                await Spark.CreateInHand(Owner, 1, CombatState);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(2m);
        }
    }
}