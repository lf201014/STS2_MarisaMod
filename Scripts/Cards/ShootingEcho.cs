using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Cards
{
    public class ShootingEcho : AbstractMarisaCard
    {
        public ShootingEcho() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

        protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8m, ValueProp.Move)];

        private bool _back2Hand = false;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target);
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

            _back2Hand = false;
            var cardModel =
            (await CardSelectCmd.FromHand(choiceContext, Owner, new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1), null, this)).FirstOrDefault();
            if (cardModel != null)
            {
                if (cardModel is Burn && !cardPlay.IsAutoPlay)
                {
                    _back2Hand = true;
                }
                await CardCmd.Exhaust(choiceContext, cardModel);
            }
        }

        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (cardPlay.Card == this && _back2Hand)
            {
                await CardPileCmd.Add(this, Owner.PlayerCombatState.Hand);
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(4m);
        }

    }
}