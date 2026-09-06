using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Cards;

public class ShootTheMoon : AbstractAmplifiedCard
{
    public ShootTheMoon() : base(1, 1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Concat([
        // new CalculationBaseVar(8m),
        // new ExtraDamageVar(5m),
        // new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) => card is AbstractAmplifiedCard { IsAmplified: true } ? 1 : 0)
        new DamageVar(12, ValueProp.Move),
        new DamageVar("DamageAmplified", 12, ValueProp.Move)
    ]);

    protected override void OnUpgrade()
    {
        // DynamicVars.CalculationBase.UpgradeValueBy(3);
        // DynamicVars.ExtraDamage.UpgradeValueBy(2);
        DynamicVars.Damage.UpgradeValueBy(4);
        DynamicVars["DamageAmplified"].UpgradeValueBy(4);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var damage = AmplifiedInPlay ? DynamicVars["DamageAmplified"].BaseValue : DynamicVars.Damage.BaseValue;
        await DamageCmd.Attack(damage).FromCard(this,cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        if (Owner.RunState.CurrentRoom!.RoomType != MegaCrit.Sts2.Core.Rooms.RoomType.Boss && AmplifiedInPlay)
        {
            var pows = cardPlay.Target.Powers.Where(p => p.TypeForCurrentAmount == PowerType.Buff && p is not ReattachPower).ToList();

            if (IsUpgraded)
            {
                foreach (var pow in pows)
                {
                    await PowerCmd.Remove(pow);
                }
            }
            else
            {
                if (pows.Count != 0)
                {
                    var pow = pows.TakeRandom(1, Owner.RunState.Rng.CombatCardSelection).FirstOrDefault();
                    await PowerCmd.Remove(pow);
                }
            }
        }
    }
}