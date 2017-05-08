using System;
using System.Collections.Generic;
using spaar.ModLoader;
using spaar.ModLoader.UI;
using Selectors;
using UnityEngine;

namespace spaar.Mods.Automatron.Actions
{
  public class ActionChangeSliderValue : BlockSpecificAction
  {
    public override string Title { get; set; } = "Change Slider Value";

    private int selectedSlider = -1;
    private float value = 1f;
    private string textFieldText = "";

    public override void Create(ConfigureDoneCallback cb, HideGUICallback hideCb)
    {
      base.Create(cb, hideCb);

      textFieldText = value.ToString();
    }

    private void UpdateTitle()
    {
      if (Game.IsSimulating) return;

      var block = GetBlock();
      if (block == null)
      {
        Title = "Change Slider Value";
      }
      else
      {
        if (selectedSlider == -1)
        {
          Title = "Change Slider Value";
        }
        else
        {
          Title = "Set slider " + (selectedSlider + 1) + " of " + block.name
            + " to " + value.ToString("F");
        }
      }

    }

    public override void Trigger()
    {
      if (selectedSlider == -1) return;

      var mSlider = GetBlock()?.Sliders[selectedSlider];

      if (mSlider == null)
      {
        Debug.LogWarning("Could not find block with Guid " + block + "!");
        return;
      }

      mSlider.Value = value;

      if (GetBlock() is BallastWeightController && mSlider.Key == "mass")
      {
        // Ballasts don't update their weight while simulating by default
        GetBlock().Rigidbody.mass = value;
      }
    }

    protected override void DoWindow(int id)
    {
      GUILayout.Label("Block: " + block);
      if (GUILayout.Button("Select Block"))
      {
        SelectBlock();
      }

      var sliders = GetBlock()?.Sliders;
      if (block != default(Guid))
      {
        if (sliders == null)
        {
          GUILayout.Label("Warning:\nBlock does not exist\n", WarningStyle);
          UpdateTitle();
        }
        else
        {
          if (sliders.Count > 0 && selectedSlider == -1)
          {
            selectedSlider = 0;
            UpdateTitle();
          }
          if (sliders.Count == 0)
          {
            GUILayout.Label("Warning:\nNo sliders on this block!", WarningStyle);
            selectedSlider = -1;
            UpdateTitle();
          }
          else
          {
            GUILayout.Label("Slider:");
            GUILayout.BeginHorizontal();
            for (int i = 0; i < sliders.Count; i++)
            {
              if (GUILayout.Button((i + 1).ToString(), i == selectedSlider
                ? Elements.Buttons.Default
                : Elements.Buttons.Disabled))
              {
                selectedSlider = i;
                UpdateTitle();
              }
              if (i % 3 == 0 && i != 0)
              {
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
              }
            }
            GUILayout.EndHorizontal();

            GUILayout.Label("Change to:");
            float max, min;
            if (sliders != null && selectedSlider != -1)
            {
              max = sliders[selectedSlider].Max;
              min = sliders[selectedSlider].Min;
            }
            else
            {
              max = 5.0f;
              min = 0.0f;
            }
            float newValue = GUILayout.HorizontalSlider(value, min, max);
            if (newValue != value)
            {
              textFieldText = newValue.ToString();
              value = newValue;
              UpdateTitle();
            }
            textFieldText = GUILayout.TextField(textFieldText);
            if (Event.current.isKey && Event.current.keyCode == KeyCode.Return)
            {
              if (float.TryParse(textFieldText, out newValue))
              {
                value = newValue;
                UpdateTitle();
              }
              else
              {
                textFieldText = value.ToString();
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
      var data = "Change Slider Value?" +
                 "{block:" + block +
                 ",selectedSlider:" + selectedSlider +
                 ",value:" + value + "}";
      return data;
    }

    public override void Load(string data)
    {
      base.Load(data);

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
          case "selectedSlider":
            selectedSlider = int.Parse(val);
            break;
          case "value":
            value = float.Parse(val);
            break;
        }
      }

      textFieldText = value.ToString();
      UpdateTitle();
    }
  }
}