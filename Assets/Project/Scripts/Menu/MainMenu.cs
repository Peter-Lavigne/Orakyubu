using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum MenuState { Main, Extras, Credits };

public class MainMenu : MonoBehaviour {
  public MenuState menuState;
  public GameObject newGameText;
  public GameObject continueGameText;
  public GameObject exitButton;
  public List<GameObject> gameObjectsToMoveDownInWebGl;

  private void Awake() {
    if (System.IO.File.Exists(Application.persistentDataPath + "/Progress.json")) {
      newGameText.SetActive(false);
      continueGameText.SetActive(true);
    }

    if (Application.platform == RuntimePlatform.WebGLPlayer) {
      exitButton.SetActive(false);
      for (int index = 0; index < gameObjectsToMoveDownInWebGl.Count; index++) {
        GameObject gameObject = gameObjectsToMoveDownInWebGl[index];
        gameObject.transform.localPosition = new Vector3(
          gameObject.transform.localPosition.x,
          gameObject.transform.localPosition.y - 40f * (index + 1),
          gameObject.transform.localPosition.z
        );
      }
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
