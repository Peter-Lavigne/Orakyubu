using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class PauseMenu : MonoBehaviour {
  public GameObject pauseMenu;
  public GameObject pauseMenuItems;
  public GameObject settingsMenuItems;
  public Controller controller;
  public GameObject levelSelect;
  private bool settingsOnlyMode = false;
  public PauseEvent pauseEvent = new PauseEvent();
  public Settings settings;
  public GameObject resetConfirmation;

  public bool paused = false;

  public void ShowResetConfirmation() {
    settingsMenuItems.SetActive(false);
    resetConfirmation.SetActive(true);
  }

  public void HideResetConfirmation() {
    settingsMenuItems.SetActive(true);
    resetConfirmation.SetActive(false);
  }

  public void ResetAllProgress() {
    settings.Cancel();
    File.Delete(Application.persistentDataPath + "/Progress.json");
    LoadMainMenu();
  }

  public void TogglePaused() {
    if (!paused) {
      SetPaused(true);
    } else {
      if (settingsMenuItems.active) {
        settings.Cancel();
      }
      SetPaused(false);
    }
  }

  public void Resume() {
    SetPaused(false);
  }

  void SetPaused(bool _paused) {
    paused = _paused;
    if (paused) {
      ShowOrHideLevelSelect();
      resetConfirmation.SetActive(false);
    }
    pauseMenu.SetActive(paused);
    Time.timeScale = paused ? 0f : 1f;
    if (settingsOnlyMode) {
      SelectSettings();
    } else {
      DeselectSettings();
    }
    pauseEvent.Invoke(paused);
  }

  private bool InPuzzleMode() {
    if (controller == null) return false;
    return controller.GetControlMode() == ControlMode.Puzzle;
  }

  private void ShowOrHideLevelSelect() {
    bool show = Globals.mode == Mode.main && InPuzzleMode();
    levelSelect.SetActive(show);
    for (int i = 0; i < pauseMenuItems.transform.childCount - (show ? 0 : 1); i++) {
      int index = show ? i : (i == 0 ? 0 : i + 1);
      RectTransform rt = pauseMenuItems.transform.GetChild(index).transform.GetComponent<RectTransform>();
      rt.sizeDelta = new Vector2(
        rt.sizeDelta.x,
        show ? 80 : 100
      );
      rt.anchoredPosition = new Vector2(
        rt.anchoredPosition.x,
        (show ? 120f : 100f) - i * (show ? 80f : 100f)
      );
    }
  }

  public void OnClickLevelSelect() {
    controller.SetControlMode(ControlMode.LevelSelect);
    SetPaused(false);
  }

  public void LoadMainMenu() {
    Time.timeScale = 1f;
    SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
  }

  public void SelectSettings() {
    pauseMenuItems.SetActive(false);
    settingsMenuItems.SetActive(true);
  }

  public void DeselectSettings() {
    if (settingsOnlyMode) {
      SetPaused(false);
    } else {
      pauseMenuItems.SetActive(true);
      settingsMenuItems.SetActive(false);
    }
  }

  public void OpenInSettingsOnlyMode() {
    settingsOnlyMode = true;
    SetPaused(true);
  }
}
