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

    private void UpdateTitle()
    {
      Title = "Delay " + textFieldText + (secondsMode ? " seconds" : " frames");
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
        UpdateTitle();
      }
      textFieldText = GUILayout.TextField(textFieldText);
      if (Event.current.isKey && Event.current.keyCode == KeyCode.Return)
      {
        int newVal;
        if (int.TryParse(textFieldText, out newVal))
        {
          count = newVal;
          UpdateTitle();
        }
        else
        {
          textFieldText = count.ToString();
          UpdateTitle();
        }
      }

      GUILayout.Label("Mode:");
      GUILayout.BeginHorizontal();
      if (GUILayout.Button("seconds", secondsMode
        ? Elements.Buttons.Default
        : Elements.Buttons.Disabled))
      {
        secondsMode = true;
        UpdateTitle();
      }
      if (GUILayout.Button("frames", secondsMode
        ? Elements.Buttons.Disabled
        : Elements.Buttons.Default))
      {
        secondsMode = false;
        UpdateTitle();
      }
      GUILayout.EndHorizontal();

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
      UpdateTitle();
    }
  }
}