using System;
using System.Collections.Generic;
using spaar.ModLoader;
using spaar.ModLoader.UI;
using spaar.Mods.Automatron.Actions;
using UnityEngine;

namespace spaar.Mods.Automatron
{
  public abstract class Action
  {
    public delegate void ConfigureDoneCallback();
    public delegate void HideGUICallback(bool hide);

    public static Dictionary<string, Type> ActionTypes
      = new Dictionary<string, Type>
      {
        {"Change Toggle Value", typeof(ActionChangeToggleValue)},
        {"Change Slider Value", typeof(ActionChangeSliderValue)},
        {"Change Limit Value", typeof(ActionChangeLimitValue)},
        {"Delay", typeof(ActionDelay)},
        {"Press Key", typeof(ActionPressKey)}
      };

    public static Action Deserialize(string data)
    {
      var parts = data.Split('?');
      var typeString = "";
      var actionData = "";
      if (parts.Length == 0)
      {
        return null;
      }
      else if (parts.Length == 1)
      {
        typeString = parts[0];
      }
      else if (parts.Length == 2)
      {
        typeString = parts[0];
        actionData = parts[1];
      }

      if (typeString == "")
      {
        return null;
      }

      var type = ActionTypes[typeString];
      var action = (Action)Activator.CreateInstance(type);
      action.Load(actionData);

      return action;
    }

    protected Rect windowRect = new Rect(1200, 200, 300, 500);
    protected int windowId = Util.GetWindowID();
    protected bool configuring = false;
    private bool hidden = false;

    protected ConfigureDoneCallback currentCallback;
    protected HideGUICallback currentHideCallback;

    public abstract void Trigger();
    public virtual void Update() { }

    public abstract string Title { get; set; }
    protected abstract void DoWindow(int id);
    protected abstract void Close();

    public abstract string Serialize();

    protected GUIStyle WarningStyle;

    public virtual void Create(ConfigureDoneCallback cb, HideGUICallback hideCb)
    {
      WarningStyle = new GUIStyle(Elements.Labels.Default)
      {
        normal = { textColor = Elements.Colors.LogWarning }
      };

      configuring = true;
      currentCallback = cb;
      currentHideCallback = hideCb;
    }

    public virtual void Load(string data)
    {
      WarningStyle = new GUIStyle(Elements.Labels.Default)
      {
        normal = { textColor = Elements.Colors.LogWarning }
      };
    }

    public virtual void Configure(ConfigureDoneCallback cb,
      HideGUICallback hideCb)
    {
      configuring = true;
      currentCallback = cb;
      currentHideCallback = hideCb;
    }

    public virtual void StopConfiguring()
    {
      if (!configuring) return;
      Close();
    }

    public virtual void Hide(bool hide)
    {
      hidden = hide;
      currentHideCallback(hide);
    }

    public virtual void OnGUI()
    {
      if (!configuring || hidden) return;
      GUI.skin = ModGUI.Skin;
      windowRect = GUI.Window(windowId, windowRect, DoWindow, Title);
    }
  }
}
