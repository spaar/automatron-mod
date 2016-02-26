using System;
using spaar.ModLoader.UI;
using UnityEngine;

namespace spaar.Mods.Automatron.Actions
{
  public class ActionDelay : Action
  {
    public override string Title { get; set; } = "Delay";

    public int count = 0;
    public bool secondsMode = true;
    private string textFieldText = "";

    public override void Create(ConfigureDoneCallback cb, HideGUICallback hideCb)
    {
      base.Create(cb, hideCb);

      textFieldText = count.ToString();
    }

    public override void Trigger()
    {
      // Delay has to be handled as a special case in AutomatronBlock,
      // so it's not necessary to do anything here.
    }

    protected override void DoWindow(int id)
    {
      GUILayout.Label("Delay:");
      float newValue = GUILayout.HorizontalSlider(count, 0f, 10f);
      if (count != (int)Math.Round(newValue))
      {
        count = (int)Math.Round(newValue);
        textFieldText = count.ToString();
      }
      textFieldText = GUILayout.TextField(textFieldText);
      if (Event.current.isKey && Event.current.keyCode == KeyCode.Return)
      {
        int newVal;
        if (int.TryParse(textFieldText, out newVal))
        {
          count = newVal;
        }
        else
        {
          textFieldText = count.ToString();
        }
      }

      GUILayout.Label("Mode:");
      GUILayout.BeginHorizontal();
      if (GUILayout.Button("seconds", secondsMode
        ? Elements.Buttons.Default
        : Elements.Buttons.Disabled))
      {
        secondsMode = true;
      }
      if (GUILayout.Button("frames", secondsMode
        ? Elements.Buttons.Disabled
        : Elements.Buttons.Default))
      {
        secondsMode = false;
      }
      GUILayout.EndHorizontal();

      GUILayout.FlexibleSpace();

      if (GUILayout.Button("Save"))
      {
        configuring = false;
        currentCallback();
      }

      GUI.DragWindow();
    }

    public override string Serialize()
    {
      var data = "Delay?" +
                 "{count:" + count + "}" +
                 ",secondsMode:" + secondsMode + "}";
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
          case "count":
            count = int.Parse(val);
            break;
          case "secondsMode":
            secondsMode = bool.Parse(val);
            break;
        }
      }

      textFieldText = count.ToString();
    }
  }
}