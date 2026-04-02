using marisamod.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace marisamod.Scripts.Cards
{
    public class SporeBomb : AbstractAmplifiedCard
    {
        public SporeBomb() : base(0, 1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
        {
        }

        protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Concat([
            new DynamicVar("Power", 2)
        ]);

        protected override void OnUpgrade()
        {
            DynamicVars["Power"].UpgradeValueBy(1);
        }

        protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<VulnerablePower>()
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (IsAmplified)
            {
                foreach (Creature enemy in CombatState.HittableEnemies)
                {
                    await PowerCmd.Apply<VulnerablePower>(enemy, DynamicVars["Power"].IntValue, base.Owner.Creature, this);
                }
            }
            else
            {
                ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
                await PowerCmd.Apply<VulnerablePower>(cardPlay.Target, DynamicVars["Power"].IntValue, Owner.Creature, this);
            }
        }
    }
}