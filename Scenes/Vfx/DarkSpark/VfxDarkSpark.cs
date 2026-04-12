using Godot;
using marisamod.Scenes.Vfx.FinalSpark;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.TestSupport;

namespace marisamod.Scenes.Vfx.DarkSpark;

public partial class VfxDarkSpark : Node2D
{
    [Export] public ShaderMaterial? DarkMaterial { get; set; }
    private const float FlareTime = 0.3f;
    private const float BeamTime = 0.5f;
    private const float FadeTime = 0.2f;
    public static float VfxTime =>(FlareTime+BeamTime+FadeTime);
    private static readonly string ScenePath = "res://Scenes/Vfx/DarkSpark/VfxDarkSpark.tscn";
    
    
    public override void _Ready()
    {
        //TaskHelper.RunSafely(PlaySequence());
    }
    
    public enum  BeamPhase
    {
        Idle,
        Flare,
        Beam,
        Fade,
    }
    public BeamPhase Phase = BeamPhase.Idle;
    
    public double Time;
    public override void _Process(double delta)
    {
        Rotation += MathF.PI / 20f * (float)delta / VfxTime;
        if (Phase == BeamPhase.Idle)
        {
            StarShader.SetShaderParameter("progress",0);
            BeamShader.SetShaderParameter("progress",0);
            Phase  = BeamPhase.Flare;
        }else if (Phase == BeamPhase.Flare)
        {
            float progress = (float) Time/FlareTime;
            Time += delta;
            float starProgress = progress/2f ;
            StarShader.SetShaderParameter("progress",starProgress);//前半段是闪光阶段
            if (progress >= 1)
            {
                Phase = BeamPhase.Beam;
                Time = 0;
                NGame.Instance?.ScreenShake(ShakeStrength.Medium, ShakeDuration.Normal);
            }
                
        }else if (Phase == BeamPhase.Beam)
        {
            float progress = (float) Time/BeamTime;
            Time += delta;
            float starProgress = Mathf.Min(1.0f,0.5f + progress*2.0f) ;
            StarShader.SetShaderParameter("progress",starProgress);
            float beamProgress = Mathf.Min(1.0f,progress*4.0f);
            BeamShader.SetShaderParameter("progress",beamProgress);
            if (progress >= 1)
            {
                Phase = BeamPhase.Fade;
                Time = 0;
            }
        }else if (Phase == BeamPhase.Fade)
        {
            float progress = (float)Time/FadeTime;
            Time += delta;
            StarShader.SetShaderParameter("progress",1.0 - progress/2.0);
            BeamShader.SetShaderParameter("progress",1.0 - progress);
            if (progress >= 1)
                this.QueueFreeSafely();
        }
    }
    
    public static VfxDarkSpark? Create(Creature owner, Creature target)
    {
        if (TestMode.IsOn)
        {
            return null;
        }
        NCreature? nCreature = NCombatRoom.Instance?.GetCreatureNode(owner);
        NCreature? nCreature2 = NCombatRoom.Instance?.GetCreatureNode(target);
        if (nCreature2 != null && nCreature != null)
        {
            Vector2 vfxSpawnPosition = nCreature.VfxSpawnPosition;
            Player? player = owner.Player;
            if (player is { Character: Defect })
            {
                vfxSpawnPosition += Defect.EyelineOffset;
            }
            return Create(vfxSpawnPosition, nCreature2.VfxSpawnPosition);
        }
        return null;
    }
    public static VfxDarkSpark? Create(Vector2 start, Vector2 target)
    {
        if (TestMode.IsOn)
        {
            return null;
        }
        VfxDarkSpark instance = GD.Load<PackedScene>(ScenePath).Instantiate<VfxDarkSpark>();
        float dir = (target - start).Angle() - MathF.PI / 40f;
        instance.Rotation = dir;
        instance.Position =  start;
        return instance;
    }
    public void SetRainbowRatio(float ratio)
    {
        BeamShader.SetShaderParameter("rainbow_ratio",ratio);
    }
    
    
    private ShaderMaterial? _beamShader;
    private ShaderMaterial BeamShader => _beamShader  ??= (ShaderMaterial)(GetNode<MeshInstance2D>("beam").Material);
    
    private ShaderMaterial? _starShader;
    private ShaderMaterial StarShader => _starShader  ??= (ShaderMaterial)(GetNode<Sprite2D>("star").Material);
}