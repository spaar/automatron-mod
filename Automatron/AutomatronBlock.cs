using System;
using System.Collections.Generic;
using System.Diagnostics;
using spaar.ModLoader;
using spaar.ModLoader.UI;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace spaar.Mods.Automatron
{
  public class AutomatronBlock : BlockScript
  {
    private MKey activateKey;
    private MToggle configureToggle;

    private bool configuring = false;
    private bool configuringAction = false;

    private Rect windowRect = new Rect(500, 200, 300, 300);
    private int windowId = Util.GetWindowID();

    private Rect addActionWindowRect = new Rect(850, 200, 200, 300);
    private int addActionWindowId = Util.GetWindowID();
    private bool addingAction = false;

    private bool hidden = false;

    private List<Action> actions = new List<Action>();

    public override void SafeAwake()
    {
      activateKey = AddKey("Activate", "activate", KeyCode.B); // TODO: default?
      configureToggle = AddToggle("Configure", "configure", false);
      configureToggle.Toggled += Toggled;
    }

    private void Toggled(bool active)
    {
      configuring = active;
    }

    protected override void OnSimulateUpdate()
    {
      if (activateKey.IsPressed)
      {
        Debug.Log("Key pressed");
        foreach (var action in actions)
        {
          action.Trigger();
        }
      }
    }

    protected override void BuildingUpdate()
    {
      foreach (var action in actions)
      {
        action.Update();
      }
    }

    public override void OnLoad(BlockXDataHolder stream)
    {
      LoadMapperValues(stream);

      if (stream.HasKey("automatron-actions"))
      {
        DeserializeActions(stream.ReadString("automatron-actions"));
      }
    }

    public override void OnSave(BlockXDataHolder stream)
    {
      SaveMapperValues(stream);

      stream.Write("automatron-actions", SerializeActions());
    }

    private string SerializeActions()
    {
      var data = "";
      foreach (var action in actions)
      {
        data += action.Serialize() + ";";
      }
      if (data.Length > 0)
      {
        data = data.Substring(0, data.Length - 1); // Strip extra ; at the end
      }
      return data;
    }

    private void DeserializeActions(string data)
    {
      var serializedActions = data.Split(';');
      foreach (var serializedAction in serializedActions)
      {
        actions.Add(Action.Deserialize(serializedAction));
      }
    }

    private void OnGUI()
    {
      if (!configuring) return;

      GUI.skin = ModGUI.Skin;

      if (!hidden)
      {
        windowRect = GUI.Window(windowId, windowRect, DoWindow,
          "Automatron Configuration");

        if (addingAction)
        {
          addActionWindowRect = GUI.Window(addActionWindowId,
            addActionWindowRect, DoAddActionWindow, "Add Action");
        }
      }

      if (configuringAction)
      {
        foreach (var action in actions)
        {
          action.OnGUI();
        }
      }
    }

    private void DoWindow(int id)
    {
      foreach (var action in actions)
      {
        if (GUILayout.Button(action.Title))
        {
          if (!configuringAction)
          {
            configuringAction = true;
            action.Configure(ConfigureActionDone, HideGUI);
          }
        }
      }

      if (GUILayout.Button("Add action"))
      {
        addingAction = true;
      }

      GUI.DragWindow();
    }

    private void DoAddActionWindow(int id)
    {
      GUILayout.Label("Choose type:");
      foreach (var key in Action.ActionTypes.Keys)
      {
        if (GUILayout.Button(key))
        {
          addingAction = false;
          var actionType = Action.ActionTypes[key];
          var action = (Action) Activator.CreateInstance(actionType);
          configuringAction = true;
          action.Create(ConfigureActionDone, HideGUI);
          actions.Add(action);
        }
      }

      GUI.DragWindow();
    }

    private void ConfigureActionDone()
    {
      configuringAction = false;
    }

    private void HideGUI(bool hide)
    {
      hidden = hide;
    }

  }
}
