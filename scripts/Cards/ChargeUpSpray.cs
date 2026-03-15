using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MarisaMod.scripts.Cards.Abstract;

namespace MarisaMod.scripts.Cards
{
    public class ChargeUpSpray : AbstractMarisaModCard
    {
        public ChargeUpSpray() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
        {
        }

        public override string PortraitPath => $"res://img/cards/ChargeUpSpray_p.png";

        protected override IEnumerable<DynamicVar> CanonicalVars => [
            new DamageVar(8m, ValueProp.Move),
            new EnergyVar(1)
            ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_lightning").Execute(choiceContext);
            if (Owner.Creature.HasPower<ChargeUpPower>() && Owner.Creature.GetPower<ChargeUpPower>()!.Amount > 8)
            {
                await CardPileCmd.Draw(choiceContext, 2, Owner);
                await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(4m);
        }
    }
}