using marisamod.Scripts.PatchesNModels;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Cards
{
    public class RefractionSpark : AbstractAmplifiedCard
    {
        public RefractionSpark() : base(1, 1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }


        protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Concat([
            new CalculationBaseVar(4m),
            new ExtraDamageVar(3m),
            new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) => card is AbstractAmplifiedCard { IsAmplified: true } ? 1 : 0)
            ]);

        protected override HashSet<CardTag> CanonicalTags => [MarisaCardTags.Spark];

        protected override void OnUpgrade()
        {
            DynamicVars.CalculationBase.UpgradeValueBy(1m);
            DynamicVars.ExtraDamage.UpgradeValueBy(1m);
        }


        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target);
            var dmgCmd = await DamageCmd.Attack(DynamicVars.CalculatedDamage).FromCard(this).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

            var add = dmgCmd.Results.Sum(x => x.UnblockedDamage);

            if (add > 0)
                foreach (CardModel card in Owner.PlayerCombatState!.Hand.Cards.Where(x => x.Tags.Contains(MarisaCardTags.Spark)).ToArray())
                {
                    if (card.DynamicVars.ContainsKey("CalculatedDamage"))
                    {
                        card.DynamicVars.CalculationBase.UpgradeValueBy(add);
                    }
                    else if (card.DynamicVars.ContainsKey("Damage"))
                    {
                        card.DynamicVars.Damage.UpgradeValueBy(add);
                    }
                }
        }
    }
}