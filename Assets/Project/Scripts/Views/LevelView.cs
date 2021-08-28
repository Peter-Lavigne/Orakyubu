using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelView : ViewModel<Level>, Clickable {
  public GameObject light;
  private Vector3 startPosition;
  private float hoverOffset;
  public float hoverAmplitude;
  public float hoverSpeed;

  private static Quaternion RotationFromLevelPosition(Vector3 position) {
    if (position.z == -4) {
      return Quaternion.Euler(0, 90f, -90f);
    }

    throw new System.ArgumentException("Invalid level position");
  }

  public override void Setup(Level _model, View _view) {
    base.Setup(_model, _view);

    view.controller.controlModeChangedEvent.AddListener(HandleControlModeChangedEvent);

    float xAngle = 360f / ControllerHelpers.levelOrder.Count();

    float distance = 100;

    if (ControllerHelpers.levelOrder.Any(list => list.Contains(model.name))) {
      int xIndex = ControllerHelpers.LevelIndexX(model.name);
      int yIndex = ControllerHelpers.LevelIndexY(model.name);
      int levelsCount = ControllerHelpers.LevelCountInColumn(model.name);
      Quaternion rotation = Quaternion.Euler(
        0f,
        -xAngle * xIndex + Random.Range(-5.0f, 5.0f),
        0f
      );
      gameObject.transform.position = rotation * (Vector3.back * distance) + (Vector3.up * ((-16 * yIndex + 8 * (levelsCount - 1) + Random.Range(-1.0f, 1.0f))));
      gameObject.transform.localRotation = rotation;
    } else {
      Quaternion rotation = Quaternion.Euler(0, -xAngle * -1, 0f);
      gameObject.transform.localRotation = rotation;
      gameObject.transform.position = rotation * (Vector3.back * distance);
    }
    startPosition = transform.localPosition;
    hoverOffset = Random.Range(0f, 100f);
  }

  void HandleControlModeChangedEvent(ControlMode mode) {
    if (mode == ControlMode.Edit) {
      model.Uncomplete();
      light.GetComponent<Light>().color = Color.white;
    }
  }
  
  public override bool Animating() {
    return false;
  }

  bool ShowLevel() {
    if (Globals.mode == Mode.editor) return true;
    if (model.name == "Intro to 2D") return true;
    
    return ControllerHelpers
      .PreviousLevels(model.name)
      .Select(levelName => view.model.levels.FirstOrDefault(l => l.name == levelName))
      .Any(l => l.IsComplete());
  }

  public override void Rerender() {
    if (!ShowLevel()) {
      gameObject.SetActive(false);
      return;
    }

    gameObject.SetActive(true);

    if (model.IsComplete()) {
      light.GetComponent<Light>().color = Color.green;
    } else {
      light.GetComponent<Light>().color = Color.white;
    }
  }

  public void OnClick() {
    view.controller.OnClickLevel(model);
  }

  void Update() {
    transform.localPosition = startPosition + new Vector3(0.0f, Mathf.Sin((Time.time + hoverOffset) * hoverSpeed) * hoverAmplitude, 0.0f);
  }
}
