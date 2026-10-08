using marisamod.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Cards;

public class FourOfASpark : AbstractMarisaCard
{
    public FourOfASpark() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10m, ValueProp.Move)
    ];

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        var attackCommand = await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        var amount = attackCommand.Results.SelectMany(r => r).Sum(r => r.UnblockedDamage);
        if (amount > 0)
        {
            await PowerCmd.Apply<HoukaiPower>(choiceContext, cardPlay.Target, amount, Owner.Creature, this);
        }
        
        IEnumerable<Creature> enumerable = from c in CombatState?.GetTeammatesOf(base.Owner.Creature)
            where c is { IsAlive: true, IsPlayer: true }
            select c;
        foreach (var creature in enumerable)
        {
            if (creature.Player != null)
            {
                var card = CombatState?.CreateCard<TabooSpark>(creature.Player);
                var result = await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Draw, base.Owner, CardPilePosition.Random);
                if (LocalContext.IsMe(creature))
                {
                    CardCmd.PreviewCardPileAdd(result);
                }
            }
        }
    }
}