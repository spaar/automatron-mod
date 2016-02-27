using System;
using System.Collections.Generic;
using spaar.ModLoader;
using spaar.ModLoader.UI;
using UnityEngine;

namespace spaar.Mods.Automatron.Actions
{
  public class ActionChangeToggleValue : BlockSpecificAction
  {
    public override string Title { get; set; } = "Change Toggle Value";

    private bool toggle = false;
    private int selectedToggle = -1;
    private bool changeTo = false;

    private void UpdateTitle()
    {
      var block = GetBlock();
      if (block == null)
      {
        Title = "Change Toggle Value";
      }
      else
      {
        if (selectedToggle == -1)
        {
          Title = "Change Toggle Value";
        }
        else
        {
          if (toggle)
          {
            Title = "Toggle toggle " + selectedToggle + " of " + block.name;
          }
          else
          {
            Title = " Set toggle " + selectedToggle + " of " + block.name
              + " to " + (changeTo ? "on" : "off");
          }
        }
      }
    }

    public override void Trigger()
    {
      if (selectedToggle == -1) return;

      var mToggle = GetBlock()?.Toggles[selectedToggle];

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

    protected override void DoWindow(int id)
    {
      GUILayout.Label("Block: " + block);
      if (GUILayout.Button("Select Block"))
      {
        SelectBlock();
      }

      if (block != default(Guid))
      {
        var toggles = GetBlock()?.Toggles;

        if (toggles == null)
        {
          GUILayout.Label("Warning:\nBlock does not exist\n");
          UpdateTitle();
        }
        else
        {
          if (toggles.Count > 0 && selectedToggle == -1)
          {
            selectedToggle = 0;
            UpdateTitle();
          }
          if (toggles.Count == 0)
          {
            GUILayout.Label("Warning:\nNo toggles on this block!");
            selectedToggle = -1;
            UpdateTitle();
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
                UpdateTitle();
              }
              if (i % 3 == 0 && i != 0)
              {
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
              }
            }
            GUILayout.EndHorizontal();

            GUILayout.Label("Mode:");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Toggle", toggle
              ? Elements.Buttons.Default
              : Elements.Buttons.Disabled))
            {
              toggle = true;
              UpdateTitle();
            }
            if (GUILayout.Button("Set", toggle
              ? Elements.Buttons.Disabled
              : Elements.Buttons.Default))
            {
              toggle = false;
              UpdateTitle();
            }
            GUILayout.EndHorizontal();


            if (!toggle)
            {
              GUILayout.Label("Change to:");
              var style = changeTo
                ? Elements.Buttons.Default
                : Elements.Buttons.Disabled;

              if (GUILayout.Button(changeTo ? "On" : "Off", style))
              {
                changeTo = !changeTo;
                UpdateTitle();
              }
            }
          }
        }
      }

      GUILayout.FlexibleSpace();

      if (GUILayout.Button("Save"))
      {
        Close();
      }

      GUI.DragWindow();
    }

    protected override void Close()
    {
      configuring = false;
      currentCallback();
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

      UpdateTitle();
    }
  }
}