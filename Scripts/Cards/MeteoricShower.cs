using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Cards
{
    public class MeteoricShower : AbstractMarisaCard
    {
        public MeteoricShower() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.RandomEnemy)
        {
        }

        protected override bool HasEnergyCostX => true;

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(7m, ValueProp.Move)
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            var exhaustNum = ResolveEnergyXValue();
            if (IsUpgraded)
                exhaustNum++;

            if (exhaustNum > 0)
            {
                var hitCount = 0;
                foreach (CardModel item in
                         await CardSelectCmd.FromHand(prefs: new CardSelectorPrefs(SelectionScreenPrompt, 0, exhaustNum), context: choiceContext, player: Owner, filter: null, source: this))
                {
                    if (item is Burn)
                        hitCount++;
                    await CardCmd.Exhaust(choiceContext, item);
                    hitCount++;
                }

                await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(hitCount).FromCard(this)
                    .TargetingRandomOpponents(CombatState!)
                    .WithHitFx("vfx/vfx_attack_slash")
                    .Execute(choiceContext);
            }
        }
    }
}