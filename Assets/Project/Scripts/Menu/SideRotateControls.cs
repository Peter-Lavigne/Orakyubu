using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class SideRotateControls : MonoBehaviour {
  public GameObject xRotateControls;
  public GameObject yRotateControls;
  public Model model;
  public Controller controller;

  void ShowOrHideControls() {
    xRotateControls.SetActive(
      controller.GetControlMode() == ControlMode.LevelSelect &&
      model.levels.FirstOrDefault(l => l.name == "Intro to 3D").IsComplete()
    );
    yRotateControls.SetActive(
      controller.GetControlMode() == ControlMode.LevelSelect &&
      model.finalLevelAvailable
    );
  }

  void Awake() {
    ShowOrHideControls();
    controller.controlModeChangedEvent.AddListener(OnControlModeChangedEvent);
  }

  private void OnControlModeChangedEvent(ControlMode mode) {
    ShowOrHideControls();
  }
}
