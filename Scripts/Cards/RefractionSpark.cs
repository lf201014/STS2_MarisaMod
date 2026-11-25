using Godot;
using marisamod.Scenes.Vfx.SparkProjectile;
using marisamod.Scripts.PatchesNModels;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace marisamod.Scripts.Cards
{
    public class RefractionSpark : AbstractAmplifiedCard
    {
        public RefractionSpark() : base(1, 1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
        {
        }


        protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars.Concat([
            // new CalculationBaseVar(5m),
            // new ExtraDamageVar(3m),
            // new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) => card is AbstractAmplifiedCard { IsAmplified: true } ? 1 : 0)
            new DamageVar(5, ValueProp.Move),
            new DamageVar("DamageAmplified", 8, ValueProp.Move)
        ]);

        protected override HashSet<CardTag> CanonicalTags => [MarisaCardTags.Spark];

        protected override void OnUpgrade()
        {
            // DynamicVars.CalculationBase.UpgradeValueBy(2m);
            // DynamicVars.ExtraDamage.UpgradeValueBy(2m);
            DynamicVars.Damage.UpgradeValueBy(2);
            DynamicVars["DamageAmplified"].UpgradeValueBy(4);
        }


        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await base.OnPlay(choiceContext, cardPlay);
            ArgumentNullException.ThrowIfNull(cardPlay.Target);
            var enemyPos = (NCombatRoom.Instance?.GetCreatureNode(cardPlay.Target)!).VfxSpawnPosition;
            var vfx = VfxSparkProjectile.Create(this, new Vector4(1f, 1f, 1f, 1.0f),
                NCombatRoom.Instance.GetCreatureNode(cardPlay.Target)!);
            var damage = !AmplifiedInPlay ? DynamicVars.Damage.BaseValue : DynamicVars["DamageAmplified"].BaseValue;
            var dmgCmd = await DamageCmd.Attack(damage).FromCard(this,cardPlay).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .BeforeDamage(async delegate
                {
                    NCombatRoom.Instance?.AddChildSafely(vfx);
                    await Cmd.Wait(vfx.VfxTime);
                })
                //.WithHitVfxNode((Creature t) => )
                .Execute(choiceContext);

            var add = dmgCmd.Results.SelectMany(r=>r).Sum(r=>r.UnblockedDamage);

            if (add > 0)
            {
                NCreature player = NCombatRoom.Instance.GetCreatureNode(Owner.Creature)!;
                float hue = 0;
                var cards = Owner.PlayerCombatState!.Hand.Cards.Where(x => x.Tags.Contains(MarisaCardTags.Spark)).ToArray();
                foreach (var card in cards)
                {
                    // if (card.DynamicVars.ContainsKey("CalculatedDamage"))
                    // {
                    //     card.DynamicVars.CalculationBase.UpgradeValueBy(add);
                    // }
                    // else if (card.DynamicVars.ContainsKey("Damage"))
                    // {
                    //     card.DynamicVars.Damage.UpgradeValueBy(add);
                    // 
                    
                    float startDir;
                    startDir = GodotObject.IsInstanceValid(vfx) ? vfx.Velocity.Angle() : ( enemyPos - player.VfxSpawnPosition).Angle();
                    Vector2 targetPos = player.VfxSpawnPosition;
                    if (LocalContext.IsMe(Owner))
                    {
                        NCard targetCardNode = NCombatRoom.Instance?.Ui.Hand.GetCard(card)!;
                        Vector2? local =
                            NCombatRoom.Instance?.CombatVfxContainer.GetGlobalTransformWithCanvas().AffineInverse() *
                            targetCardNode.GetGlobalTransformWithCanvas().Origin;
                        if (local != null)
                            targetPos = local.Value;
                    }
                    
                    NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(CreatePowerUpSpark(player, enemyPos, targetPos,hue,add ,startDir));
                    PowerUp.UpgradeCardDamage(card, add);
                    hue += 1f / cards.Length;
                }
            }
        }
        public static Vector4 Hsv2Rgba(float h,float s,float v)
        {
            if (s <= float.Epsilon)
            {
                return new Vector4(v, v, v,1);
            }
            var hue = h * 360f;

            var huePrime = hue / 60f;
            var i = Mathf.FloorToInt(huePrime);
            var f = huePrime - i;
            
            var p = v * (1f - s);
            var q = v * (1f - s * f);
            var t = v * (1f - s * (1f - f));
            return i switch
            {
                0 => new Vector4(v, t, p, 1),
                1 => new Vector4(q, v, p, 1),
                2 => new Vector4(p, v, t, 1),
                3 => new Vector4(p, q, v, 1),
                4 => new Vector4(t, p, v, 1),
                _ => new Vector4(v, p, q, 1)
            };
        }
        public static VfxSparkProjectile CreatePowerUpSpark(NCreature player,Vector2 enemyPos, Vector2 targetPos,float hue,int damage,float dir)
        {
            VfxSparkProjectile vfx =VfxSparkProjectile.Create();
            vfx.SetAnimationParament(chasingSpeedMixMin:0.2f,chasingDirMixMin:0.2f);
            vfx.SetColor(Hsv2Rgba(hue,0.8f,1));
            vfx.PlayerOwner= player;
            vfx.Position = enemyPos;
            vfx.VelocityInit(dir,spread:0.1f*Mathf.Pi);
            vfx.StartDamping();
            vfx.Target = targetPos;
            vfx.NoIdle = true;
            vfx.ApplySizeFromDamage(damage);
            return vfx;
        }
    }
}