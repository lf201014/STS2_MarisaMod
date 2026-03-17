using marisamod.scripts.Cards.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.scripts.Cards
{
    public class AlicesGift : AbstractAmplifiedCard
    {
        public AlicesGift() : base(0, 1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
        {
        }
        //public override string PortraitPath => $"res://img/cards/GiftDoll_v2_p.png";

        public override IEnumerable<CardKeyword> CanonicalKeywords => base.CanonicalKeywords.Concat([CardKeyword.Retain]);

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new CalculationBaseVar(0m),
            new ExtraDamageVar(5m),
            new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _)=>card is AbstractAmplifiedCard { IsAmplified: true } ? 3 : 1),
            new EnergyVar(1)
        ];

        

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target);
            await DamageCmd.Attack(DynamicVars.CalculatedDamage).FromCard(this).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(2m);
        }
    }
}