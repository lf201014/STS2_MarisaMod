using Godot;
using marisamod.Scenes.Vfx.SparkProjectile;
using marisamod.Scripts.PatchesNModels;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Cards
{
    public class MachineGunSpark : AbstractMarisaCard
    {
        public MachineGunSpark() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
        {
        }


        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(1m, ValueProp.Move),
        new RepeatVar(6)
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords => base.CanonicalKeywords.Concat([
            CardKeyword.Exhaust
        ]);
        
        protected override HashSet<CardTag> CanonicalTags => [MarisaCardTags.Spark];

        protected override void OnUpgrade()
        {
            DynamicVars["Repeat"].UpgradeValueBy(2);
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
            //NCreature? player = NCombatRoom.Instance?.GetCreatureNode(base.Owner.Creature);
            CleanVfx();
            CreateVfx();
            await Cmd.Wait(0.1f);
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(DynamicVars["Repeat"].IntValue).FromCard(this,cardPlay)
                .Targeting(cardPlay.Target)
                .WithHitVfxNode(InvokeVfx)
                .WithHitFx(null, null, "blunt_attack.mp3")
                .Execute(choiceContext);
            CleanVfx();
        }
        private List<VfxSparkProjectile> _vfx = [];
        private void CleanVfx()
        {
            if (_vfx.Count == 0) return;
            foreach (var vfx in _vfx.Where(GodotObject.IsInstanceValid)) vfx.StartFading();
            _vfx.Clear();
        }
        private void CreateVfx()
        {
            var count = DynamicVars["Repeat"].IntValue;
            _vfx = new List<VfxSparkProjectile>(count);
            var color = new Vector4(0.9f, 0.8f, 0.35f, 1.0f);
            var player = NCombatRoom.Instance?.GetCreatureNode(Owner.Creature);
            if (player == null ) return;
            for (var i = 0; i < count; i++)
            {
                var vfx = VfxSparkProjectile.Create(player!,color);
                vfx.ApplySizeFromDamage((int)DynamicVars.Damage.PreviewValue);
                _vfx.Add(vfx);
                NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(_vfx[i]);
            }
        }

        private VfxSparkProjectile? InvokeVfx(Creature target)
        {
            if (_vfx is not { Count: not 0 }) return null;
            var vfx = _vfx[0];
            if (!GodotObject.IsInstanceValid(vfx))
            {
                _vfx.RemoveAt(0);
                return null;
            }
            vfx.Target = (NCombatRoom.Instance?.GetCreatureNode(target)!).VfxSpawnPosition;
            vfx.GetParent()?.RemoveChild(vfx);
            vfx.StartChasing();
            _vfx.Remove(vfx);
            return vfx;
        }
    }
}