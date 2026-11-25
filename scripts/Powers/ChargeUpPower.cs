using BaseLib.Extensions;
using Godot;
using marisamod.Scripts.Characters;
using marisamod.Scripts.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Powers
{
    public class ChargeUpPower : AbstractMarisaPower
    {
        public int ChargeUpThreshold => Owner.Player != null && Owner.Player.Relics.Any(x => x is SimpleLauncher) ? 6 : 8;

        private const int MultCeiling = 4194304;

        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Counter;

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DynamicVar("Mult", 1m)
        ];

        private int _lastPhase;

        private bool _toBeConsumed;

        public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource,CardPlay? cardPlay)
        {
            if (!props.IsPoweredAttack_())
            {
                return 1m;
            }

            if (cardSource == null)
            {
                return 1m;
            }

            if (dealer != null && dealer != Owner && !Owner.Pets.Contains(dealer))
            {
                return 1m;
            }

            return CalculateMult();
        }

        public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
        {
            if (power == this)
            {
                DynamicVars["Mult"].BaseValue = CalculateMult();
            }

            return Task.CompletedTask;
        }

        public decimal CalculateMult()
        {
            if (Amount < ChargeUpThreshold || Owner.HasPower<OneTimeOffPower>())
            {
                return 1m;
            }

            var mult = Mathf.FloorToInt(Mathf.Pow(2, Mathf.FloorToInt(Amount / (float)ChargeUpThreshold)));
            if (mult > MultCeiling)
                mult = MultCeiling;
            if (mult != _lastPhase)
            {
                if (Owner.Player?.Character is MarisaCharacter)
                {
                    var megaAnimationState = NCombatRoom.Instance?.GetCreatureNode(Owner)?.SpineAnimation.GetAnimationState();

                    if (_lastPhase < mult)
                    {
                        var animTransName = mult switch
                        {
                            2 => "_tracks/charge_up_0to1",
                            4 => "_tracks/charge_up_1to2",
                            >= 8 => "_tracks/charge_up_2to3",
                            _ => ""
                        };
                        megaAnimationState?.SetAnimation(animTransName, loop: false, 1);
                    }

                    var animName = mult switch
                    {
                        2 => "_tracks/charged_1",
                        4 => "_tracks/charged_2",
                        >= 8 => "_tracks/charged_3",
                        _ => ""
                    };

                    megaAnimationState?.AddAnimation(animName, 0f, loop: true, 1);
                    //_ = CreatureCmd.TriggerAnim(Owner, "CHARGE_UP", 0.25f);
                }

                _lastPhase = mult;
            }

            return mult;
        }

        public override Task BeforeCardPlayed(CardPlay cardPlay)
        {
            if (cardPlay.Card.Type == CardType.Attack && Amount >= ChargeUpThreshold && !Owner.HasPower<OneTimeOffPower>() && cardPlay.Card.Owner == Owner.Player)
            {
                _toBeConsumed = true;
            }

            return Task.CompletedTask;
        }

        public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (_toBeConsumed)
            {
                decimal reduceAmount = Amount - Amount % ChargeUpThreshold;
                PowerCmd.ModifyAmount(context, this, -reduceAmount, Owner, null);
                if (Owner.Player?.Character is MarisaCharacter)
                {
                    var megaAnimationState = NCombatRoom.Instance?.GetCreatureNode(Owner)?.SpineAnimation.GetAnimationState();

                    var animName = _lastPhase switch
                    {
                        2 => "_tracks/spark_heavy_1",
                        4 => "_tracks/spark_heavy_2",
                        >= 8 => "_tracks/spark_heavy_3",
                        _ => ""
                    };
                    megaAnimationState?.SetAnimation(animName, loop: false, 1);

                    megaAnimationState?.AddAnimation("_tracks/charged_0", 0f, loop: true, 1);
                }

                _toBeConsumed = false;
            }

            _lastPhase = 1;

            _toBeConsumed = false;
            return Task.CompletedTask;
        }
    }
}