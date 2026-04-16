using Godot;
using MegaCrit.Sts2.Core.Helpers;

namespace marisamod.Scenes.Vfx.SparkProjectile
{
    public partial class SparkWithTrail : Sprite2D
    {
        public const string ScenePath = "res://scene/SparkWithTrail.tscn";
        public Vector2 Velocity = Vector2.Zero;
        public Vector2 Target;
    
        public float LifeTime = 1f;
        public float IdleTime = 0.5f;//LifeTime的前多少秒是减速阶段
        public float FadeTime = 0.2f;//LifeTime结束后还会存在多少秒

    
        public float SlowCoeff = 0.9f;
        private SparkTrail? _child = null;
        public SparkTrail Child =>_child ??= GetChildOrNull<SparkTrail>(0);
        private void UpdateMovement(double delta)
        {
            if (IdleTime > 0) //从本体射出后逐渐减速的阶段
            {
                Velocity *= SlowCoeff;
                Position += Velocity * (float)delta;
                IdleTime -= (float)delta;
                Child.LifeTime2 = IdleTime;
            }
            else //向目标移动
            {
                Child.LifeTime2 = -1;
                if (delta > LifeTime)//剩余时间不足以完成本次delta
                {
                    Position = Target;
                }
                else
                {
                    float ratio = (float)delta / LifeTime;//越靠近最后越接近1
                    Vector2 diff = Target - Position;
                    float d = diff.Length();
                    diff *= ratio; //实际应该移动的距离
                    Vector2 offset = d * ratio * Velocity.Normalized();//原来速度方向添加偏移方向
                    float mixRatio = Math.Min(ratio+0.4f,  1.0f);
                    Velocity = diff * mixRatio + offset * (1 - mixRatio);
                    Position += Velocity;//越接近末尾，偏移生效的部分越少
                }
            }
        }
    
        private void UpdateFading(double delta)
        {
            FadeTime -= (float)delta;
            Child.LifeTime2 = FadeTime;
            //ShaderMaterial mat = (ShaderMaterial)Material;
            //mat.SetShaderParameter("arm_factor",1+3*FadeTime);
            if (FadeTime < 0)
                this.QueueFreeSafely();
        }

        public override void _Process(double delta)
        {
            UpdateMovement(delta);
            LifeTime -= (float)delta;
        
            if (LifeTime < 0)
            {
                UpdateFading(delta);
            }
        }

        public static SparkWithTrail QuickInstance(Vector2 start, Vector2 target,float lifeTime = 0.5f, float idleTimeRatio = 0.6f, float size = 1f)
        {
            SparkWithTrail instance = GD.Load<PackedScene>(ScenePath).Instantiate<SparkWithTrail>(PackedScene.GenEditState.Disabled);
            instance.Position = start;
            instance.Target = target;
            instance.LifeTime = lifeTime;
            instance.IdleTime =lifeTime* idleTimeRatio;
            instance.Scale *= size;
            instance.SlowCoeff = (float)Math.Pow(0.05f, 1f / (instance.LifeTime * 60f));
            float speed = 0.8f* (target - start).Length() / instance.IdleTime;
            //随机初始速度
            //float dir = (target - start).Angle();
            //dir += 0.25f * Mathf.Pi + 1.5f* Mathf.Pi * GD.Randf();
            float dir = -Mathf.Pi * GD.Randf();
            instance.Velocity = speed * Vector2.FromAngle(dir);

            return instance;
        }
    }
}
