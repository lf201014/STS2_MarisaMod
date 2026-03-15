using MarisaMod.scripts.Cards.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MarisaMod.scripts.Cards
{
    public class DC : AbstractMarisaModCard
    {
        public DC() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
        {
        }

        public override string PortraitPath => $"res://img/cards/DC_p.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new CalculationBaseVar(0m),
        new ExtraDamageVar(8m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((CardModel card, Creature? _)=>Owner.PlayerCombatState!.DiscardPile.Cards.Count == 0 ? 2 : 1)
            ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
            await DamageCmd.Attack(DynamicVars.CalculatedDamage).FromCard(this).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.ExtraDamage.UpgradeValueBy(3m);
        }
    }
}