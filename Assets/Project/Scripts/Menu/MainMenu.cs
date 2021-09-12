﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum MenuState { Main, Extras, Credits };

public class MainMenu : MonoBehaviour {
  public MenuState menuState;
  public GameObject newGameText;
  public GameObject continueGameText;

  private void Awake() {
    if (System.IO.File.Exists(Application.persistentDataPath + "/Progress.json")) {
      newGameText.SetActive(false);
      continueGameText.SetActive(true);
    }
  }

  public void OnClickNewGame() {
    Globals.mode = Mode.main;
    SceneManager.LoadScene("Main",  LoadSceneMode.Single);
  }

  public void OnClickLevelEditor() {
    Globals.mode = Mode.editor;
    SceneManager.LoadScene("Main",  LoadSceneMode.Single);
  }

  public void OnClickExit() {
    Application.Quit();
  }

  public void OnClickExtras() {
    menuState = MenuState.Extras;
  }

  public void OnClickExtrasBack() {
    menuState = MenuState.Main;
  }

  public void OnClickCredits() {
    menuState = MenuState.Credits;
  }

  public void OnClickCreditsBack() {
    menuState = MenuState.Extras;
  }

  public void OnClickDiscord() {
    Application.OpenURL("https://discord.gg/KKDWhUNW5V");
  }

  void Start() {
    Cursor.visible = true;
    Screen.lockCursor = false;
  }
}
