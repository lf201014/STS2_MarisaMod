using Godot;
using Godot.Collections;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;

namespace marisamod.Scripts.PatchesNModels;

public partial class MehParticlesContainer : NParticlesContainer
{

	public override void _Ready()
	{
		base._Ready();
		Array<GpuParticles2D> pts = [];
		foreach (var item in GetChildren().Where(c => c is GpuParticles2D))
		{
			if (item is GpuParticles2D gpuParticles2D)
				pts.Add(gpuParticles2D);
		}
		Traverse.Create(this).Field("_particles").SetValue(pts);//new Array<GpuParticles2D>());
	}

}
