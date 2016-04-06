using System;
using spaar.ModLoader;
using spaar.ModLoader.UI;
using UnityEngine;

namespace spaar.Mods.Automatron.Actions
{
  public class ActionChangeLimitValue : BlockSpecificAction
  {
    public override string Title { get; set; } = "Change Limit Value";

    private int selectedLimit = -1;
    private float valueMin = 90f;
    private float valueMax = 90f;
    private string textFieldTextMin = "";
    private string textFieldTextMax = "";

    public override void Create(ConfigureDoneCallback cb, HideGUICallback hideCb)
    {
      base.Create(cb, hideCb);

      textFieldTextMin = valueMin.ToString();
      textFieldTextMax = valueMax.ToString();
    }

    private void UpdateTitle()
    {
      if (Game.IsSimulating) return;

      var block = GetBlock();
      if (block == null)
      {
        Title = "Change Limit Value";
      }
      else
      {
        if (selectedLimit == -1)
        {
          Title = "Change Limit Value";
        }
        else
        {
          Title = "Set limit " + (selectedLimit + 1) + " of " + block.name
            + " to " + valueMin.ToString("F") + ", " + valueMax.ToString("F");
        }
      }
    }

    public override void Trigger()
    {
      if (selectedLimit == -1) return;

      var mLimit = GetBlock()?.Limits[selectedLimit];

      if (mLimit == null)
      {
        Debug.LogWarning("Could not find block with Guid " + block + "!");
        return;
      }

      mLimit.Min = valueMin;
      mLimit.Max = valueMax;
    }

    protected override void DoWindow(int id)
    {
      GUILayout.Label("Block: " + block);
      if (GUILayout.Button("Select Block"))
      {
        SelectBlock();
      }

      var limits = GetBlock()?.Limits;
      if (block != default(Guid))
      {
        if (limits == null)
        {
          GUILayout.Label("Warning:\nBlock does not exist\n", WarningStyle);
          UpdateTitle();
        }
        else
        {
          if (limits.Count > 0 && selectedLimit == -1)
          {
            selectedLimit = 0;
            UpdateTitle();
          }
          if (limits.Count == 0)
          {
            GUILayout.Label("Warning:\nNo limits on this block!", WarningStyle);
            selectedLimit = -1;
            UpdateTitle();
          }
          else
          {
            GUILayout.Label("Limits:");
            GUILayout.BeginHorizontal();
            for (int i = 0; i < limits.Count; i++)
            {
              if (GUILayout.Button((i + 1).ToString(), i == selectedLimit
                  ? Elements.Buttons.Default
                  : Elements.Buttons.Disabled))
              {
                selectedLimit = i;
                UpdateTitle();
              }
              if (i % 3 == 0 && i != 0)
              {
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
              }
            }
            GUILayout.EndHorizontal();

            float maxValue;
            if (limits != null && selectedLimit != -1)
            {
              maxValue = limits[selectedLimit].MaxValue;
            }
            else
            {
              maxValue = 180f;
            }

            GUILayout.Label("Minimum:");
            float newValue = GUILayout.HorizontalSlider(valueMin, 0.0f, maxValue);
            if (newValue != valueMin)
            {
              textFieldTextMin = newValue.ToString();
              valueMin = newValue;
              UpdateTitle();
            }
            textFieldTextMin = GUILayout.TextField(textFieldTextMin);
            if (Event.current.isKey && Event.current.keyCode == KeyCode.Return)
            {
              if (float.TryParse(textFieldTextMin, out newValue))
              {
                valueMin = newValue;
                UpdateTitle();
              }
              else
              {
                textFieldTextMin = valueMin.ToString();
                UpdateTitle();
              }
            }

            GUILayout.Label("Maximum:");
            newValue = GUILayout.HorizontalSlider(valueMax, 0.0f, maxValue);
            if (newValue != valueMax)
            {
              textFieldTextMax = newValue.ToString();
              valueMax = newValue;
              UpdateTitle();
            }
            textFieldTextMax = GUILayout.TextField(textFieldTextMax);
            if (Event.current.isKey && Event.current.keyCode == KeyCode.Return)
            {
              if (float.TryParse(textFieldTextMax, out newValue))
              {
                valueMax = newValue;
                UpdateTitle();
              }
              else
              {
                textFieldTextMax = valueMax.ToString();
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
      var data = "Change Limit Value?" +
                 "{block:" + block +
                 ",selectedLimit:" + selectedLimit +
                 ",valueMin:" + valueMin +
                 ",valueMax:" + valueMax + "}";
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
          case "selectedLimit":
            selectedLimit = int.Parse(val);
            break;
          case "valueMin":
            valueMin = float.Parse(val);
            break;
          case "valueMax":
            valueMax = float.Parse(val);
            break;
        }
      }

      textFieldTextMin = valueMin.ToString();
      textFieldTextMax = valueMax.ToString();
      UpdateTitle();
    }
  }
}
