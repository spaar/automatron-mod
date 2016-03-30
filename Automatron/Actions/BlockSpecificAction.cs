using System;
using spaar.ModLoader;
using UnityEngine;

namespace spaar.Mods.Automatron.Actions
{
  public abstract class BlockSpecificAction : Action
  {
    protected Guid block;
    protected bool selectingBlock = false;

    private Rect selectingInfoRect = new Rect(200, 800, 500, 100);
    private int selectingInfoId = Util.GetWindowID();

    protected BlockBehaviour GetBlock()
    {
      for (int i = 0; i < Machine.Active().BuildingBlocks.Count; i++)
      {
        if (Machine.Active().BuildingBlocks[i].Guid == block)
        {
          return Machine.Active().Blocks[i];
        }
      }
      return null;
    }

    public override void OnGUI()
    {
      base.OnGUI();

      if (selectingBlock)
      {
        selectingInfoRect = GUI.Window(selectingInfoId, selectingInfoRect,
          DoSelectingInfoWindow, "Selecting Block");
      }
    }

    public override void Update()
    {
      if (selectingBlock)
      {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
          selectingBlock = false;
          Hide(false);
        }
        if (Input.GetMouseButtonDown(1)) // Right click
        {
          RaycastHit hit;
          if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition)
            , out hit))
          {
            var obj = hit.transform;
            var b = obj.GetComponent<BlockBehaviour>();
            if (b != null)
            {
              block = b.Guid;

              selectingBlock = false;
              Hide(false);
            }
          }
        }
      }
    }

    protected void SelectBlock()
    {
      Hide(true);
      selectingBlock = true;
    }

    private void DoSelectingInfoWindow(int id)
    {
      GUILayout.Label("Select a block by hovering the mouse over it and " +
                      "right-clicking.\nCancel by pressing Escape.");
      GUI.DragWindow();
    }
  }
}