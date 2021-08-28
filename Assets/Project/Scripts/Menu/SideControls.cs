using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SideControls : MonoBehaviour {
  public Controller controller;
  public GameObject menuButton;
  public GameObject undoButton;
  public GameObject restartButton;
  public PauseMenu pauseMenu;

  void Awake() {
    ShowButtons();
    controller.controlModeChangedEvent.AddListener(HandleControlModeChangedEvent);
    pauseMenu.pauseEvent.AddListener(OnPauseEvent);
  }

  private void OnPauseEvent(bool paused) {
    menuButton.SetActive(!paused);
    if (paused) {
      restartButton.SetActive(false);
      undoButton.SetActive(false);
    } else {
      ShowButtons();
    }
  }

  void HandleControlModeChangedEvent(ControlMode mode) {
    ShowButtons();
  }

  void ShowButtons() {
    bool show = controller.GetControlMode() == ControlMode.Puzzle;
    restartButton.SetActive(show);
    undoButton.SetActive(show);
    menuButton.SetActive(
      controller.GetControlMode() != ControlMode.Credits
    );
  }
}
