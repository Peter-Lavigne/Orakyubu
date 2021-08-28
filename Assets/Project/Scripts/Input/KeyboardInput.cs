using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class KeyboardInput : MonoBehaviour {
  public MainInput mainInput;
  public PauseMenu pauseMenu;

  void Update() {
    if (mainInput.controller.GetControlMode() == ControlMode.Credits) return;

    if (Input.GetKeyDown(KeyCode.Escape)) {
      pauseMenu.TogglePaused();
      return;
    }

    if (pauseMenu.paused) return;

    if (
      Input.GetKeyDown(KeyCode.LeftShift) ||
      Input.GetKeyDown(KeyCode.RightShift)
    ) {
      mainInput.SetMoveCameraButtonHeld(true);
    }
    if (
      Input.GetKeyUp(KeyCode.LeftShift) ||
      Input.GetKeyUp(KeyCode.RightShift)
    ) {
      mainInput.SetMoveCameraButtonHeld(false);
    }

    if (Input.GetKey(KeyCode.R)) {
      mainInput.Reset();
    } else if (Input.GetKey(KeyCode.Z)) {
      mainInput.Undo();
    } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
      mainInput.SetEditObject(EditObject.Remove);
    } else if (Input.GetKeyDown(KeyCode.Alpha5)) {
      mainInput.CreateBox3d();
    } else if (Input.GetKeyDown(KeyCode.Alpha6)) {
      mainInput.SetEditObject(EditObject.Player2D);
    } else if (Input.GetKeyDown(KeyCode.Alpha7)) {
      mainInput.SetEditObject(EditObject.Box2D);
    } else if (Input.GetKeyDown(KeyCode.Alpha8)) {
      mainInput.SetEditObject(EditObject.Goal2D);
    } else if (Input.GetKeyDown(KeyCode.Alpha9)) {
      mainInput.SetEditObject(EditObject.Wall2D);
    } else if (Input.GetKey(KeyCode.W)) {
      mainInput.Move(Direction.Up);
    } else if (Input.GetKey(KeyCode.S)) {
      mainInput.Move(Direction.Down);
    } else if (Input.GetKey(KeyCode.A)) {
      mainInput.Move(Direction.Left);
    } else if (Input.GetKey(KeyCode.D)) {
      mainInput.Move(Direction.Right);
    } else if (Input.GetKey(KeyCode.UpArrow)) {
      mainInput.Move(Direction.Up);
    } else if (Input.GetKey(KeyCode.DownArrow)) {
      mainInput.Move(Direction.Down);
    } else if (Input.GetKey(KeyCode.LeftArrow)) {
      mainInput.Move(Direction.Left);
    } else if (Input.GetKey(KeyCode.RightArrow)) {
      mainInput.Move(Direction.Right);
    }
  }
}
