using System;
using System.Collections.Generic;
using spaar.ModLoader;
using spaar.ModLoader.UI;
using UnityEngine;

namespace spaar.Mods.Automatron.Actions
{
  public class ActionChangeToggleValue : Action
  {
    public override string Title { get; set; } = "Change Toggle Value";

    private Guid block;
    private bool toggle = false;
    private int selectedToggle = -1;
    private bool changeTo = false;
    private bool selectingBlock = false;

    private Rect selectingInfoRect = new Rect(200, 800, 500, 100);
    private int selectingInfoId = Util.GetWindowID();

    public override void Trigger()
    {
      MToggle mToggle = null;

      for (int i = 0; i < Machine.Active().BuildingBlocks.Count; i++)
      {
        if (Machine.Active().BuildingBlocks[i].Guid == block)
        {
          if (selectedToggle >= 0)
          {
            mToggle = Machine.Active().Blocks[i].Toggles[selectedToggle];
          }
        }
      }

      if (mToggle == null)
      {
        Debug.LogWarning("Could not find block with Guid " + block + "!");
        return;
      }

      if (toggle)
      {
        mToggle.IsActive = !mToggle.IsActive;
      }
      else
      {
        mToggle.IsActive = changeTo;
      }
    }

    public override void OnGUI()
    {
      base.OnGUI();

      if (selectingBlock)
      {
        selectingInfoRect = GUI.Window(selectingInfoId, selectingInfoRect,
          DoSelectingInfoWindow, "Selecting Block");
      }
    }

    public override void Update()
    {
      if (selectingBlock)
      {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
          selectingBlock = false;
          Hide(false);
        }
        if (Input.GetMouseButtonDown(1)) // Right click
        {
          RaycastHit hit;
          if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition)
            , out hit))
          {
            var obj = hit.transform;
            var b = obj.GetComponent<BlockBehaviour>();
            if (b != null)
            {
              block = b.Guid;

              selectingBlock = false;
              Hide(false);
            }
          }
        }
      }
    }

    protected override void DoWindow(int id)
    {
      GUILayout.Label("Block: " + block);
      if (GUILayout.Button("Select Block"))
      {
        Hide(true);
        selectingBlock = true;
      }

      if (block != default(Guid))
      {
        List<MToggle> toggles = null;
        foreach (var b in Machine.Active().Blocks)
        {
          if (b.Guid == block)
          {
            toggles = b.Toggles;
            break;
          }
        }

        if (toggles == null)
        {
          GUILayout.Label("Warning:\nBlock does not exist\n");
        }
        else
        {
          if (toggles.Count > 0 && selectedToggle == -1)
          {
            selectedToggle = 0;
          }
          if (toggles.Count == 0)
          {
            GUILayout.Label("Warning:\nNo toggles on this block!");
            selectedToggle = -1;
          }
          else
          {
            GUILayout.Label("Toggle:");
            GUILayout.BeginHorizontal();
            for (int i = 0; i < toggles.Count; i++)
            {
              if (GUILayout.Button(i.ToString(), i == selectedToggle
                ? Elements.Buttons.Default
                : Elements.Buttons.Disabled))
              {
                selectedToggle = i;
              }
              if (i%3 == 0 && i != 0)
              {
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
              }
            }
            GUILayout.EndHorizontal();
          }
        }
      }

      GUILayout.Label("Mode:");
      if (GUILayout.Button(toggle ? "Toggle" : "Set"))
      {
        toggle = !toggle;
      }

      if (!toggle)
      {
        GUILayout.Label("Change to:");
        var style = changeTo
          ? Elements.Buttons.Default
          : Elements.Buttons.Disabled;

        if (GUILayout.Button(changeTo ? "On" : "Off", style))
        {
          changeTo = !changeTo;
        }
      }

      GUILayout.FlexibleSpace();

      if (GUILayout.Button("Save"))
      {
        configuring = false;
        currentCallback();
      }

      GUI.DragWindow();
    }

    private void DoSelectingInfoWindow(int id)
    {
      GUILayout.Label("Select a block by hovering the mouse over it and " +
                      "right-clicking.\nCancel by pressing Escape.");
      GUI.DragWindow();
    }

    public override string Serialize()
    {
      var data = "Change Toggle Value?" +
                 "{block:" + block + ",toggle:" + toggle +
                 ",selectedToggle:" + selectedToggle + ",changeTo:" + changeTo
                 + "}";
      return data;
    }

    public override void Load(string data)
    {
      data = data.Replace("{", "").Replace("}", "");
      var pairs = data.Split(',');
      foreach (var pair in pairs)
      {
        var key = pair.Split(':')[0];
        var val = pair.Split(':')[1];

        switch (key)
        {
          case "block":
            block = new Guid(val);
            break;
          case "toggle":
            toggle = bool.Parse(val);
            break;
          case "selectedToggle":
            selectedToggle = int.Parse(val);
            break;
          case "changeTo":
            changeTo = bool.Parse(val);
            break;
        }
      }
    }
  }
}