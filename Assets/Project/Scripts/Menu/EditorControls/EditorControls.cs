using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EditorControls : MonoBehaviour {
  public GameObject editorControls;
  public Model model;
  public Controller controller;
  public Text exportText;
  public Text importText;
  public Animator exportMessageAnimator;
  public Animator importErrorAnimator;
  public GameObject exitPlayModeButton;
  public PauseMenu pauseMenu;

  private string importErrorText = "Failed to Load from Clipboard";

  void Awake() {
    editorControls.SetActive(
      Globals.mode == Mode.editor
    );
    pauseMenu.pauseEvent.AddListener(OnPauseEvent);
  }

  private void OnPauseEvent(bool paused) {
    editorControls.SetActive(!paused && Globals.mode == Mode.editor);
  }

  public void Export() {
    GUIUtility.systemCopyBuffer = JsonUtility.ToJson(model.levels[0]);
    exportText.text = "Level Copied to Clipboard";
    exportMessageAnimator.Start();
  }

  public void Import() {
    try {
      Level level = JsonUtility.FromJson<Level>(GUIUtility.systemCopyBuffer);
      if (level.box3ds == null) {
        importText.text = importErrorText;
        importErrorAnimator.Start();
      } else {
        model.Import(level);
      }
    } catch (System.ArgumentException e) {
      importText.text = importErrorText;
      importErrorAnimator.Start();
    }
  }

  public void AddCube() {
    controller.CreateBox3d();
  }

  void SetPlayMode(bool playMode) {
    exitPlayModeButton.SetActive(playMode);
    for (int i = 0; i < editorControls.transform.childCount; i++) {
      GameObject child = editorControls.transform.GetChild(i).gameObject;
      if (child != exitPlayModeButton) {
        child.SetActive(
          !playMode
        );
      }
    }

    controller.SetControlMode(playMode ? ControlMode.Puzzle : ControlMode.Edit);
  }

  public void EnterPlayMode() {
    SetPlayMode(true);
  }

  public void ExitPlayMode() {
    SetPlayMode(false);
  }

  Dictionary<int, EditObject> editObjectMap = new Dictionary<int, EditObject>{
    { 0, EditObject.Player2D },
    { 1, EditObject.Box2D },
    { 2, EditObject.Goal2D },
    { 3, EditObject.Wall2D },
    { 4, EditObject.Remove },
    { 5, EditObject.RemoveCube },
    { 6, EditObject.MoveCubeUp },
    { 7, EditObject.MoveCubeDown },
    { 8, EditObject.MoveCubeAway },
  };

  public void SetEditMode(int mode) {
    controller.SetEditObject(editObjectMap[mode]);
  }

  void Update() {
    if (importErrorAnimator.Animating()) {
      importErrorAnimator.Update(
        (_) => {},
        () => {
          importText.text = "Import";
        }
      );
    }
    if (exportMessageAnimator.Animating()) {
      exportMessageAnimator.Update(
        (_) => {},
        () => {
          exportText.text = "Export";
        }
      );
    }
  }
}
