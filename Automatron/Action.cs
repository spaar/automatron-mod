using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
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
      = new Dictionary<string, Type>()
      {
        {"Change Toggle Value", typeof(ActionChangeToggleValue)},
        {"Change Slider Value", typeof(ActionChangeSliderValue)}
      };

    public static Action Deserialize(string data)
    {
      var typeString = data.Split('?')[0];
      var actionData = data.Split('?')[1];

      var type = ActionTypes[typeString];
      var action = (Action)Activator.CreateInstance(type);
      action.Load(actionData);

      return action;
    }

    protected Rect windowRect = new Rect(850, 200, 270, 500);
    protected int windowId = Util.GetWindowID();
    protected bool configuring = false;
    private bool hidden = false;

    protected ConfigureDoneCallback currentCallback;
    protected HideGUICallback currentHideCallback;

    public abstract void Trigger();
    public virtual void Update() { }

    public abstract string Title { get; set; }
    protected abstract void DoWindow(int id);

    public abstract string Serialize();
    public abstract void Load(string data);

    public virtual void Create(ConfigureDoneCallback cb, HideGUICallback hideCb)
    {
      configuring = true;
      currentCallback = cb;
      currentHideCallback = hideCb;
    }

    public virtual void Configure(ConfigureDoneCallback cb,
      HideGUICallback hideCb)
    {
      configuring = true;
      currentCallback = cb;
      currentHideCallback = hideCb;
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
