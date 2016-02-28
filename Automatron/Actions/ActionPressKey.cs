using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using spaar.ModLoader;
using spaar.ModLoader.UI;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace spaar.Mods.Automatron.Actions
{
  public class ActionPressKey : Action
  {
    public override string Title { get; set; } = "Press Key";

    private static Process keySim = null;
    private static StreamWriter keySimInput = null;
    private static StreamReader keySimOutput = null;

    private string keys = "";
    private static string heldKeys = "";

    // 0 = Press, 1 = Hold, 2 = Release
    private int mode = -1;

    public static void StartKeySim()
    {
      var javaPath = Environment.GetEnvironmentVariable("JAVA_HOME")
                     + "\\bin\\javaw.exe";
      var simulatorJar = Application.dataPath + "/Mods/KeySimulator.jar";
      keySim = new Process();
      keySim.StartInfo.FileName = javaPath;
      keySim.StartInfo.Arguments = "-jar " + simulatorJar;
      keySim.StartInfo.UseShellExecute = false;
      keySim.StartInfo.RedirectStandardInput = true;
      keySim.StartInfo.RedirectStandardOutput = true;
      keySim.Start();

      keySimInput = new StreamWriter(keySim.StandardInput.BaseStream,
        Encoding.ASCII);
      keySimOutput = new StreamReader(keySim.StandardOutput.BaseStream,
        Encoding.ASCII);
    }

    public static void StopKeySim()
    {
      if (keySim == null) return;
      keySimInput.Close();
      keySim.CloseMainWindow();
      keySim.Close();
    }

    public override void Create(ConfigureDoneCallback cb, HideGUICallback hideCb)
    {
      base.Create(cb, hideCb);

      Game.OnSimulationToggle += OnSimulationToggle;
    }

    private void UpdateTitle()
    {
      if (keys == "")
      {
        Title = "Press Key";
      }
      else if (mode == 0)
      {
        Title = "Press " + keys;
      }
      else if (mode == 1)
      {
        Title = "Hold " + keys;
      }
      else if (mode == 2)
      {
        Title = "Release " + keys;
      }
    }

    public override void Trigger()
    {
      var prefix = "";
      if (mode == 0)
      {
        prefix = "p:";
      }
      else if (mode == 1)
      {
        prefix = "h:";
        heldKeys += keys;
      }
      else if (mode == 2)
      {
        prefix = "r:";
        foreach (char c in keys)
        {
          heldKeys = heldKeys.Replace(c.ToString(), "");
        }
      }

      keySimInput.WriteLine(prefix + keys);
      keySimInput.Flush();
    }

    private void OnSimulationToggle(bool active)
    {
      if (!active && heldKeys != "")
      {
        keySimInput.WriteLine("r:" + heldKeys);
        keySimInput.Flush();
        heldKeys = "";
      }
    }

    private bool ValidateKeys()
    {
      if (keys == "") return true;

      // Disregard possible trailing comma
      var myKeys = keys.TrimEnd(',');

      keySimInput.WriteLine("c:" + myKeys);
      keySimInput.Flush();

      var returnVal = keySimOutput.ReadLine();
      return returnVal == "v";
    }

    protected override void DoWindow(int id)
    {
      GUILayout.Label("Keys:");
      var newKeys = GUILayout.TextField(keys);
      if (newKeys != keys)
      {
        keys = newKeys;
        UpdateTitle();
      }

      var valid = ValidateKeys();

      if (!valid)
      {
        GUILayout.Label("Warning: Invalid keys");
      }

      GUILayout.Label("Mode:");
      if (mode == -1) mode = 0;

      GUILayout.BeginHorizontal();
      if (GUILayout.Button("Press", mode == 0 ? Elements.Buttons.Default
                                              : Elements.Buttons.Disabled))
      {
        mode = 0;
        UpdateTitle();
      }
      if (GUILayout.Button("Hold", mode == 1 ? Elements.Buttons.Default
                                              : Elements.Buttons.Disabled))
      {
        mode = 1;
        UpdateTitle();
      }
      if (GUILayout.Button("Release", mode == 2 ? Elements.Buttons.Default
                                              : Elements.Buttons.Disabled))
      {
        mode = 2;
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
      UpdateTitle();
      currentCallback();
    }

    public override string Serialize()
    {
      var data = "Press Key?"
        + "{keys:" + keys
        + ",mode:" + mode + "}";
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
          case "keys":
            keys = val;
            break;
          case "mode":
            mode = int.Parse(val);
            break;
        }
      }

      UpdateTitle();
    }
  }
}