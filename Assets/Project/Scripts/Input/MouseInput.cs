using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MouseInput : MonoBehaviour {
  public MainInput mainInput;
  public View view;
  public MainCamera camera;
  public float scrollSensitivity;
  public float dragSensitivity;
  public float dragDistanceThreshold;
  public PauseMenu pauseMenu;

  private Vector3 dragOrigin;
  private bool dragging = false;
  private Vector3 lastMousePositionDraggedFrom;
  private float marginX = 400f;
  private float marginY = 200f;
  private float sideRotateSpeed = 80f;

  void Update() {
    if (pauseMenu.paused) return;
    if (view.controller.GetControlMode() == ControlMode.Credits) return;
  
    if (Input.GetMouseButtonDown(1)) {
      lastMousePositionDraggedFrom = Input.mousePosition;
      mainInput.SetMoveCameraButtonHeld(true);
    }

    if (Input.GetMouseButtonUp(1)) {
      mainInput.SetMoveCameraButtonHeld(false);
    }

    if (Input.GetMouseButton(1)) {
      mainInput.RotatePivot((Input.mousePosition.x - lastMousePositionDraggedFrom.x) * dragSensitivity, (Input.mousePosition.y - lastMousePositionDraggedFrom.y) * dragSensitivity);
      lastMousePositionDraggedFrom = Input.mousePosition;
    } else {
      mainInput.MouseMoved((Input.mousePosition.x - lastMousePositionDraggedFrom.x) * dragSensitivity, (Input.mousePosition.y - lastMousePositionDraggedFrom.y) * dragSensitivity);
      lastMousePositionDraggedFrom = Input.mousePosition;
    }

    if (Input.GetMouseButtonDown(0)) {
      view.Clicked();
    } else if (Input.GetMouseButton(0)) {
      view.MouseDown();
    }

    mainInput.Zoom(Input.mouseScrollDelta.y * scrollSensitivity);

    if (!(
      (Input.mousePosition.x < 100f && Input.mousePosition.y > Screen.height - 100f) || // on menu button
      Input.mousePosition.x > Screen.width || // out of bounds right
      Input.mousePosition.x < 0f || // out of bounds left
      Input.mousePosition.y < 0f || // out of bounds bottom
      Input.mousePosition.y > Screen.height // out of bounds top
    )) {
      marginX = Screen.width / 5f;
      if (Input.mousePosition.x > Screen.width - marginX) {
        mainInput.RotateSides((Input.mousePosition.x - (Screen.width - marginX)) / marginX * sideRotateSpeed * Time.deltaTime, 0);
      } else if (Input.mousePosition.x < marginX) {
        mainInput.RotateSides((marginX - Input.mousePosition.x) / marginX * -sideRotateSpeed * Time.deltaTime, 0);
      }

      marginY = Screen.height / 6f;
      if (Input.mousePosition.y > Screen.height - marginY) {
        mainInput.RotateSides(0, (Input.mousePosition.y - (Screen.height - marginY)) / marginY * sideRotateSpeed * Time.deltaTime);
      } else if (Input.mousePosition.y < marginY) {
        mainInput.RotateSides(0, (marginY - Input.mousePosition.y) / marginY * -sideRotateSpeed * Time.deltaTime);
      }
    }
  }
}
