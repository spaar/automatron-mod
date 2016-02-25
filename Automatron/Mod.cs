using System;
using System.Collections.Generic;
using Blocks;
using spaar.ModLoader;
using TheGuysYouDespise;
using UnityEngine;

namespace spaar.Mods.Automatron
{
  public class AutomatronMod : BlockMod
  {
    public override string Name { get; } = "automatron";
    public override string DisplayName { get; } = "Automatron";
    public override string Author { get; } = "spaar";

    public override Version Version { get; } = new Version(1, 0, 0);
    public override string VersionExtra { get; } = "";
    public override string BesiegeVersion { get; } = "v0.25";

    public override bool CanBeUnloaded { get; } = false;
    public override bool Preload { get; } = false;

    private Block automatronBlock = new Block()
      .ID(410)
      .BlockName("Automatron")
      .Obj(new List<Obj>
      {
        new Obj("automatron.obj", "automatron.png",
          new VisualOffset(new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(0f, 0f, 0.5f),
            new Vector3(0f, 0f, 0f)))
      })
      .IconOffset(new Icon(
        new Vector3(1.3f, 1.3f, 1.3f),
        new Vector3(-0.11f, -0.13f, 0f),
        new Vector3(85f, 90f, 270f)))
      .Components(new[] { typeof(AutomatronBlock) })
      .Properties(new BlockProperties().SearchKeywords(new[] { "Automatron", "Automation" })) // TODO
      .Mass(1.0f) // TODO
      .ShowCollider(false)
      .CompoundCollider(new List<ColliderComposite>()
      {
        ColliderComposite.Box(new Vector3(1, 1, 1),
          new Vector3(0, 0, 0.5f),
          new Vector3(0, 0, 0))
      })
      .IgnoreIntersectionForBase()
      .NeededResources(new List<NeededResource>())
      .AddingPoints(new List<AddingPoint>());


    public override void OnLoad()
    {
      LoadBlock(automatronBlock);
    }

    public override void OnUnload()
    {

    }
  }
}
