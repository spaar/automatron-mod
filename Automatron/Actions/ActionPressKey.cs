using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Linq;
using spaar.ModLoader;
using spaar.ModLoader.UI;
using UnityEngine;
using Debug = UnityEngine.Debug;
using System.Collections.Generic;

namespace spaar.Mods.Automatron.Actions
{
  public class ActionPressKey : Action
  {
    public override string Title { get; set; } = "Press Key";

    private static Process keySim = null;
    private static StreamWriter keySimInput = null;
    private static StreamReader keySimOutput = null;

    private static bool hasError = false;
    private static string error = "";

    private string keys = "";
    private static HashSet<string> heldKeys = new HashSet<string>();

    // 0 = Press, 1 = Hold, 2 = Release
    private int mode = -1;

    public static void StartKeySim()
    {
      try
      {
        var simulatorJar = Application.dataPath + "/Mods/KeySimulator.jar";
        keySim = new Process();
        keySim.StartInfo.FileName = "javaw";
        keySim.StartInfo.Arguments = "-jar " + '"' + simulatorJar + '"';
        keySim.StartInfo.UseShellExecute = false;
        keySim.StartInfo.RedirectStandardInput = true;
        keySim.StartInfo.RedirectStandardOutput = true;
        keySim.Start();
        keySimInput = new StreamWriter(keySim.StandardInput.BaseStream,
          Encoding.ASCII);
        keySimOutput = new StreamReader(keySim.StandardOutput.BaseStream,
          Encoding.ASCII);

        keySimInput.WriteLine("init");
        keySimInput.Flush();

        if (keySimOutput.ReadLine() != "ok")
        {
          throw new Exception(
            "KeySimulator did not respond with proper ok message.");
        }
      }
      catch (Exception e)
      {
        hasError = true;
        error = "Could not start Key Simulator.\nMake sure it and Java are " +
          "installed correctly.\nFurther information is printed in the console.";
        Debug.LogException(e);
      }
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
      if (keys == "") return;

      var myKeys = keys.TrimEnd(',');

      var prefix = "";
      if (mode == 0)
      {
        prefix = "p:";
      }
      else if (mode == 1)
      {
        prefix = "h:";
        var splitKeys = myKeys.Split(',');
        foreach (var c in splitKeys)
        {
          heldKeys.Add(c);
        }
      }
      else if (mode == 2)
      {
        prefix = "r:";
        var splitKeys = myKeys.Split(',');
        foreach (var c in splitKeys)
        {
          heldKeys.Remove(c);
        }
      }

      if (!hasError)
      {
        keySimInput.WriteLine(prefix + keys);
        keySimInput.Flush();
      }
    }

    private void OnSimulationToggle(bool active)
    {
      if (!active && heldKeys.Count > 0 && !hasError)
      {
        foreach (var key in new HashSet<string>(heldKeys))
        {
          Debug.Log("Releasing key: " + key);
          keySimInput.WriteLine("r:" + key);
          keySimInput.Flush();
          heldKeys.Remove(key);
        }
      }
    }

    private bool ValidateKeys()
    {
      if (keys == "" || hasError) return true;

      // Disregard possible trailing comma
      var myKeys = keys.TrimEnd(',');

      keySimInput.WriteLine("c:" + myKeys);
      keySimInput.Flush();

      var returnVal = keySimOutput.ReadLine();
      return returnVal == "v";
    }

    protected override void DoWindow(int id)
    {
      if (hasError)
      {
        GUILayout.Label("Warning: " + error, WarningStyle);
      }

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
        GUILayout.Label("Warning: Invalid keys", WarningStyle);
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
        + "{keys:" + keys.Replace(',', '_')
        + ",mode:" + mode + "}";
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
          case "keys":
            keys = val.Replace('_', ',');
            break;
          case "mode":
            mode = int.Parse(val);
            break;
        }
      }

      UpdateTitle();
      Game.OnSimulationToggle += OnSimulationToggle;
    }
  }
}