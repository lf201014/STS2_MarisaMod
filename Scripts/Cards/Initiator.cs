using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Cards;

public class Initiator : AbstractMarisaCard
{
    public Initiator() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(1, ValueProp.Move),
        new CardsVar(3),
        new EnergyVar(1)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var dmgCmd = await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this,cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        var dmg = dmgCmd.Results.SelectMany(r => r).Sum(x => x.UnblockedDamage);
        var draw = Owner.PlayerCombatState!.DrawPile.Cards.Where(x => x.Type is CardType.Attack).ToArray();
        var cards2Add = draw.TakeRandom(Mathf.Min(draw.Length, DynamicVars.Cards.IntValue), Owner.RunState.Rng.CombatCardSelection).ToArray();
        if (cards2Add.Length != 0)
        {
            foreach (var card in cards2Add)
            {
                card.EnergyCost.AddThisTurn(-DynamicVars.Energy.IntValue);
                await CardPileCmd.Add(card, PileType.Hand);
                if (dmg > 0)
                {
                    // if (card.DynamicVars.ContainsKey("CalculatedDamage"))
                    // {
                    //     card.DynamicVars.CalculationBase.UpgradeValueBy(dmg);
                    // }
                    // else if (card.DynamicVars.ContainsKey("Damage"))
                    // {
                    //     card.DynamicVars.Damage.UpgradeValueBy(dmg);
                    // }

                    // if (card is AbstractMarisaCard marisaCard)
                    //     _ = marisaCard.DoFlash();

                    PowerUp.UpgradeCardDamage(card, dmg);
                }
            }
        }
    }
}