using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Cards
{
    public class ShootTheMoon : AbstractAmplifiedCard
    {
        public ShootTheMoon() : base(1, 1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }


        protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Concat([
            new CalculationBaseVar(8m),
            new ExtraDamageVar(5m),
            new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) => card is AbstractAmplifiedCard { IsAmplified: true } ? 1 : 0)
            ]);

        protected override void OnUpgrade()
        {
            DynamicVars.CalculationBase.UpgradeValueBy(3);
            DynamicVars.ExtraDamage.UpgradeValueBy(2);
        }


        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target);
            await DamageCmd.Attack(DynamicVars.CalculatedDamage).FromCard(this).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

            if (Owner.RunState.CurrentRoom.RoomType != MegaCrit.Sts2.Core.Rooms.RoomType.Boss)
            {
                if (IsAmplified)
                {
                    foreach (PowerModel item in cardPlay.Target.Powers.Where(x => x.Type == MegaCrit.Sts2.Core.Entities.Powers.PowerType.Buff).ToArray())
                    {
                        await PowerCmd.Remove(item);
                    }
                }
                else
                {
                    var pow = cardPlay.Target.Powers.Where(x => x.Type == MegaCrit.Sts2.Core.Entities.Powers.PowerType.Buff && x is not ReattachPower).
                    TakeRandom(1, Owner.RunState.Rng.CombatCardSelection).FirstOrDefault();
                    if (pow != default)
                        await PowerCmd.Remove(pow);
                }
            }
        }
    }
}