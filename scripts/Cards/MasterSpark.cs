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
    public class MasterSpark : AbstractAmplifiedCard
    {
        public MasterSpark() : base(1, 1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
        {
        }

        public override string PortraitPath => $"res://img/cards/MasterSpark_p.png";

        protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new CalculationBaseVar(8m),
        new ExtraDamageVar(7m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((CardModel card, Creature? target) => IsAmplified ? 1 : 0),
        new EnergyVar(1)
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
            DynamicVars.CalculationBase.UpgradeValueBy(3m);
            DynamicVars.ExtraDamage.UpgradeValueBy(2m);
        }
    }
}