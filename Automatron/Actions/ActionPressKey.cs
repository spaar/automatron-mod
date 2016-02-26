﻿using System;
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

    private string keys = "";
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
      keySim.Start();

      keySimInput = new StreamWriter(keySim.StandardInput.BaseStream,
        Encoding.ASCII);
    }

    public static void StopKeySim()
    {
      if (keySim == null) return;
      keySimInput.Close();
      keySim.CloseMainWindow();
      keySim.Close();
    }

    public override void Trigger()
    {
      var prefix = "";
      if (mode == 0) prefix = "p:";
      else if (mode == 1) prefix = "h:";
      else if (mode == 2) prefix = "r:";

      keySimInput.WriteLine(prefix + keys);
      keySimInput.Flush();
    }

    protected override void DoWindow(int id)
    {
      GUILayout.Label("Keys:");
      keys = GUILayout.TextField(keys);

      GUILayout.Label("Mode:");
      if (mode == -1) mode = 0;

      GUILayout.BeginHorizontal();
      if (GUILayout.Button("Press", mode == 0 ? Elements.Buttons.Default
                                              : Elements.Buttons.Disabled))
      {
        mode = 0;
      }
      if (GUILayout.Button("Hold", mode == 1 ? Elements.Buttons.Default
                                              : Elements.Buttons.Disabled))
      {
        mode = 1;
      }
      if (GUILayout.Button("Release", mode == 2 ? Elements.Buttons.Default
                                              : Elements.Buttons.Disabled))
      {
        mode = 2;
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
    }
  }
}