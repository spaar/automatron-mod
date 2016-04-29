using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace spaar.Mods.Automatron
{
  public class KeySimulatorHeartbeat : MonoBehaviour
  {
    public StreamWriter keySimInput { get; set; }
    public StreamReader keySimOutput { get; set; }


    private void Start()
    {
      StartCoroutine(LoopCoroutine());
    }

    private IEnumerator LoopCoroutine()
    {
      while (true)
      {
        yield return new WaitForSeconds(5f);
        SendHeartbeat();
      }
    }

    public void SendHeartbeat()
    {
      keySimInput.WriteLine("ping");
    }
  }
}
