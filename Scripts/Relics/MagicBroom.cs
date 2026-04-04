using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace marisamod.Scripts.Relics
{
    public class MagicBroom : AbstractMarisaRelic
    {
        public override RelicRarity Rarity => RelicRarity.Rare;

        private bool _isActivating;

        private int _cardsPlayed;

        public override string FlashSfx => "event:/sfx/ui/relic_activate_draw";

        public override bool ShowCounter => true;

        public override int DisplayAmount
        {
            get
            {
                if (!IsActivating)
                {
                    return CardsPlayed % base.DynamicVars.Cards.IntValue;
                }
                return base.DynamicVars.Cards.IntValue;
            }
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(3)];
        private bool IsActivating
        {
            get
            {
                return _isActivating;
            }
            set
            {
                AssertMutable();
                _isActivating = value;
                UpdateDisplay();
            }
        }

        [SavedProperty]
        public int CardsPlayed
        {
            get
            {
                return _cardsPlayed;
            }
            set
            {
                AssertMutable();
                _cardsPlayed = value;
                UpdateDisplay();
            }
        }

        private void UpdateDisplay()
        {
            if (IsActivating)
            {
                base.Status = RelicStatus.Normal;
            }
            else
            {
                int intValue = base.DynamicVars.Cards.IntValue;
                base.Status = ((CardsPlayed % intValue == intValue - 1) ? RelicStatus.Active : RelicStatus.Normal);
            }
            InvokeDisplayAmountChanged();
        }

        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner == base.Owner && cardPlay.Resources.EnergySpent == 0)
            {
                CardsPlayed++;
                int intValue = base.DynamicVars.Cards.IntValue;
                if (CombatManager.Instance.IsInProgress && CardsPlayed % intValue == 0)
                {
                    _ = TaskHelper.RunSafely(DoActivateVisuals());
                    await CardPileCmd.Draw(context, 1m, base.Owner);
                }
            }
        }

        private async Task DoActivateVisuals()
        {
            IsActivating = true;
            Flash();
            await Cmd.Wait(1f);
            IsActivating = false;
        }
    }
}