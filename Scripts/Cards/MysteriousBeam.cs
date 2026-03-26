using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;

namespace marisamod.Scripts.Cards;

public class MysteriousBeam : AbstractMarisaCard
{
    public MysteriousBeam() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var card =
            CardFactory.GetForCombat(
                Owner,
                Owner.Character.CardPool.GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint).Where(c => c.Type is CardType.Attack),
                1,
                Owner.RunState.Rng.CombatCardGeneration
            ).FirstOrDefault();
        if (card != null)
        {
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, addedByPlayer: true);

            decimal damage = 0;
            if (card.DynamicVars.ContainsKey("CalculatedDamage"))
            {
                damage = card.DynamicVars.CalculatedDamage.Calculate(null);
            }
            else if (card.DynamicVars.ContainsKey("Damage"))
            {
                damage = card.DynamicVars.Damage.BaseValue;
            }
            else if (card.DynamicVars.ContainsKey("OstyDamage"))
            {
                damage = card.DynamicVars.OstyDamage.BaseValue;
            }
            else
            {
                Log.Warn(base.Id.Entry + ": attack card " + card.Id.Entry + " that did not have an appropriate damage var!");
            }

            await DamageCmd.Attack(damage).FromCard(this).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }
    }
}