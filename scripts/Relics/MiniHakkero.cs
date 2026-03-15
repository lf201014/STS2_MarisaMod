using BaseLib.Abstracts;
using MarisaMod.Scripts.PatchesNModels;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;

namespace MarisaMod.scripts.Relics
{
    [BaseLib.Utils.Pool(typeof(MarisaRelicPool))]
    public class MiniHakkero : CustomRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Starter;

        // 小图标
        public override string PackedIconPath => $"res://img/relics/withOutline/Hakkero_s.png";
        // 轮廓图标
        protected override string PackedIconOutlinePath => $"res://img/relics/withOutline/Hakkero_s.png";
        // 大图标
        protected override string BigIconPath => $"res://img/relics/withOutline/Hakkero_s.png";


        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner == Owner && CombatManager.Instance.IsInProgress)
            {
                _ = TaskHelper.RunSafely(DoActivateVisuals());
                await PowerCmd.Apply<ChargeUpPower>(Owner.Creature, 1, Owner.Creature, null);
            }
        }

        private async Task DoActivateVisuals()
        {
            Flash();
            //await Cmd.Wait(0.5f);
        }
    }
}