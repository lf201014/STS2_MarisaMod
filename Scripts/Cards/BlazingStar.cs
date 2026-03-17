using marisamod.scripts.Cards.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.scripts.Cards
{
    public class BlazingStar : AbstractAmplifiedCard
    {
        public BlazingStar() : base(2, 1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
        {
        }


        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new CalculationBaseVar(0m),
            new ExtraDamageVar(8m),
            new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) =>
                (Owner.PlayerCombatState!.Hand.Cards.Count(IsBurn) + 2) * (card is AbstractAmplifiedCard { IsAmplified: true } ? 2 : 1)),
            new EnergyVar(1)
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target);
            await DamageCmd.Attack(DynamicVars.CalculatedDamage).FromCard(this).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_blunt", null, "blunt_attack.mp3")
                .Execute(choiceContext);
        }

        protected override void OnUpgrade()
        {
            //DynamicVars.CalculationBase.UpgradeValueBy(4m);
            DynamicVars.ExtraDamage.UpgradeValueBy(2m);
        }

        private static bool IsBurn(CardModel card)
        {
            return card is Burn;
        }
    }
}