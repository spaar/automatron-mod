using System;
using System.Collections.Generic;
using Blocks;
using spaar.ModLoader;
using spaar.Mods.Automatron.Actions;
using TheGuysYouDespise;
using UnityEngine;

namespace spaar.Mods.Automatron
{
  public class AutomatronMod : BlockMod
  {
    public override string Name { get; } = "automatron";
    public override string DisplayName { get; } = "Automatron";
    public override string Author { get; } = "spaar";

    public override Version Version { get; } = new Version(1, 1, 4);
    public override string VersionExtra { get; } = "";
    public override string BesiegeVersion { get; } = "v0.35";

    public override bool CanBeUnloaded { get; } = false;
    public override bool Preload { get; } = false;

    private Block automatronBlock = new Block()
      .ID(410)
      .BlockName("Automatron")
      .Obj(new List<Obj>
      {
        new Obj("Automatron.obj", "Automatron.png",
          new VisualOffset(new Vector3(0.48f, 0.48f, 0.48f),
            new Vector3(0.0f, 0.0f, 0.5f),
            new Vector3(180f, 180f, 0f)))
      })
      .IconOffset(new Icon(
        new Vector3(1.0f, 1.0f, 1.0f),
        new Vector3(0.0f, 0f, 0f),
        new Vector3(360f, 70f, 300f)))
      .Components(new[] { typeof(AutomatronBlock) })
      .Properties(new BlockProperties().SearchKeywords(new[] { "Automatron", "Automation" }))
      .Mass(1.5f)
      .ShowCollider(false)
      .CompoundCollider(new List<ColliderComposite>
      {
        ColliderComposite.Box(new Vector3(1.00f, 1.00f, 1.00f),
          new Vector3(0, 0, 0.5f),
          new Vector3(0, 0, 0))
      })
      .IgnoreIntersectionForBase()
      .NeededResources(new List<NeededResource>())
      .AddingPoints(new List<AddingPoint>());


    public override void OnLoad()
    {
      LoadBlock(automatronBlock);

      ActionPressKey.StartKeySim();
    }

    public override void OnUnload()
    {
      Configuration.Save();
      ActionPressKey.StopKeySim();
    }
  }
}
