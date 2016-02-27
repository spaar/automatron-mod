using System;
using System.Collections.Generic;
using System.Diagnostics;
using spaar.ModLoader;
using spaar.ModLoader.UI;
using spaar.Mods.Automatron.Actions;
using UnityEngine;

namespace spaar.Mods.Automatron
{
  public class AutomatronBlock : BlockScript
  {
    private MKey activateKey;
    private MToggle configureToggle;

    private bool configuring = false;
    private bool configuringAction = false;

    private Rect windowRect = new Rect(500, 200, 500, 300);
    private int windowId = Util.GetWindowID();
    private Vector2 scrollPos = Vector2.zero;

    private Rect addActionWindowRect = new Rect(1000, 200, 200, 300);
    private int addActionWindowId = Util.GetWindowID();
    private bool addingAction = false;

    private bool hidden = false;

    private List<Action> actions = new List<Action>();

    public override void SafeAwake()
    {
      activateKey = AddKey("Activate", "activate", KeyCode.B);
      configureToggle = AddToggle("Configure", "configure", false);
      configureToggle.Toggled += Toggled;
    }

    private void Toggled(bool active)
    {
      configuring = active;

      if (active)
      {
        FindObjectOfType<KeyMapModeButton>().KeyMapOff();
        Game.AddPiece.hudOccluding = true;
        AddPiece.keyMapMode = true;
      }
      else
      {
        FindObjectOfType<KeyMapModeButton>().KeyMapOn();
        Game.AddPiece.hudOccluding = false;
        BlockMapper.Open(this);
      }
    }

    protected override void OnSimulateUpdate()
    {
      if (activateKey.IsPressed)
      {
        StartCoroutine(TriggerActions());
      }
    }

    protected System.Collections.IEnumerator TriggerActions()
    {
      foreach (var action in actions)
      {
        var delay = action as ActionDelay;
        if (delay != null)
        {
          if (delay.secondsMode)
          {
            yield return new WaitForSeconds(delay.count);
          }
          else
          {
            for (int i = 0; i < delay.count; i++)
            {
              yield return null; // Wait a frame
            }
          }
        }
        else
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

      if (configuring)
      {
        if (BlockMapper.CurrentInstance != null)
        {
          BlockMapper.Close();
          AddPiece.keyMapMode = true;
        }
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
      scrollPos = GUILayout.BeginScrollView(scrollPos);

      foreach (var action in new List<Action>(actions))
      {
        GUILayout.BeginHorizontal();
        var width = windowRect.width
                    - Elements.Windows.Default.padding.left
                    - Elements.Windows.Default.padding.right
                    - Elements.Buttons.Default.margin.left
                    - Elements.Buttons.Default.margin.right
                    - Elements.Buttons.Default.margin.right
                    - 25; // Scroll view

        var index = actions.IndexOf(action);
        if (GUILayout.Button(action.Title, GUILayout.Width(width / 2)))
        {
          if (!configuringAction)
          {
            configuringAction = true;
            action.Configure(ConfigureActionDone, HideGUI);
          }
        }
        if (GUILayout.Button("↑", index == 0 ? Elements.Buttons.Disabled
          : Elements.Buttons.Default, GUILayout.Width(width / 6)))
        {
          if (index != 0)
          {
            actions.Remove(action);
            actions.Insert(index - 1, action);
          }
        }
        if (GUILayout.Button("↓", index == actions.Count - 1
          ? Elements.Buttons.Disabled : Elements.Buttons.Default,
          GUILayout.Width(width / 6)))
        {
          if (index != actions.Count - 1)
          {
            actions.Remove(action);
            actions.Insert(index + 1, action);
          }
        }
        if (GUILayout.Button("x", GUILayout.Width(width / 6)))
        {
          actions.Remove(action);
        }
        GUILayout.EndHorizontal();
      }

      if (GUILayout.Button("Add action"))
      {
        addingAction = true;
      }

      if (GUILayout.Button("Close"))
      {
        configureToggle.IsActive = false;
      }

      GUILayout.EndScrollView();

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
          var action = (Action)Activator.CreateInstance(actionType);
          configuringAction = true;
          action.Create(ConfigureActionDone, HideGUI);
          actions.Add(action);
          scrollPos = new Vector2(float.PositiveInfinity, 0);
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
