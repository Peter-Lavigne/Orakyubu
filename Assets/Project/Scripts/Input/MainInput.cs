using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MainInput : MonoBehaviour {
  public Controller controller;
  public MainCamera mainCamera;
  public View view;
  public SideRotateControls sideRotateControls;
  public Settings settings;

  private Object2D hoveredPlayer2d = null;
  public Object2D GetHoveredPlayer2d() { return hoveredPlayer2d; }

  private Box3D hoveredBox3d = null;
  public Box3D GetHoveredBox3d() { return hoveredBox3d; }

  private bool moveCameraButtonHeld = false;
  public void SetMoveCameraButtonHeld(bool _moveCameraButtonHeld) { moveCameraButtonHeld = _moveCameraButtonHeld; }

  public void Reset() {
    if (view.levelCompletedPause.Animating()) return;
    controller.Reset();
  }

  public void CreateBox3d() { controller.CreateBox3d(); }
  public void SetEditObject(EditObject editObject) { controller.SetEditObject(editObject); }
  public void Zoom(float amount) { mainCamera.Zoom(amount); }

  float SensitivitySettingMultiplier(float amount) {
    return amount * (.2f + 1.6f * settings.mouseSensitivity) * (settings.mouseSensitivity > .75f ? (settings.mouseSensitivity - .75f) * 5f : 1f);
  }

  public void RotatePivot(float xAmount, float yAmount) {
    if (controller.GetControlMode() == ControlMode.Credits) return;
    mainCamera.RotatePivot(
      SensitivitySettingMultiplier(-yAmount) * (settings.invertY ? -1 : 1) * (controller.GetControlMode() == ControlMode.LevelSelect ? 0.5f : 1f),
      SensitivitySettingMultiplier(xAmount * (settings.invertX ? -1 : 1)) * (controller.GetControlMode() == ControlMode.LevelSelect ? 0.5f : 1f) 
    );
  }

  public void RotateSides(float xAmount, float yAmount) {
    if (controller.GetControlMode() != ControlMode.LevelSelect) return;
    if (moveCameraButtonHeld) return;
    if (sideRotateControls.xRotateControls.active == false) return;
    mainCamera.RotatePivot((sideRotateControls.yRotateControls.active == false ? 0f : yAmount) / 3f, -xAmount);
  }

  public void MouseMoved(float xAmount, float yAmount) {
    if (!moveCameraButtonHeld) return;
    if (controller.GetControlMode() == ControlMode.LevelSelect || controller.GetControlMode() == ControlMode.Credits) return;
    mainCamera.RotatePivot(
      SensitivitySettingMultiplier(-yAmount) * (settings.invertY ? -1 : 1),
      SensitivitySettingMultiplier(xAmount * (settings.invertX ? -1 : 1))
    );
  }

  public void Undo() {
    if (view.Animating()) return;
    controller.Undo();
  }

  public void Move(Direction direction) {
    if (view.Animating()) return;
    controller.Move(direction, mainCamera.FacingFace());
  }

  public void OnHoverPosition(Plane2D plane, Vector2Int position) {
    if (controller.GetControlMode() != ControlMode.Puzzle) return;
    if (!controller.GetCurrentLevel().box3ds.SelectMany(box3d => box3d.faces).Contains(plane)) return;

    Object2D player = plane.players
      .FirstOrDefault(player2d => player2d.position == position);
    if (player == null) {
      hoveredPlayer2d = null;
      hoveredBox3d = plane.box;
    } else {
      hoveredPlayer2d = player;
      hoveredBox3d = null;
    }
  }

  public void Unhover() {
    hoveredPlayer2d = null;
    hoveredBox3d = null;
  }
}
