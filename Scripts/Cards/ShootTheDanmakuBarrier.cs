using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Cards;

public class ShootTheDanmakuBarrier : AbstractMarisaCard
{
    public ShootTheDanmakuBarrier() : base(3,CardType.Attack,CardRarity.Uncommon,TargetType.AnyEnemy){}

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(24,ValueProp.Move)
    ];
    
    //public override string PortraitPath => "res://marisamod/images/cards/marisamod-test_marisa_card.png";

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        if (card == this && card.Enchantment != null)
        {
            return playCount + 1;
        }
        return base.ModifyCardPlayCount(card, target, playCount);
    }
}