using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Object2DView : ViewModel<Object2D>
{
  private float maxOpactity = 0f;
  private float randomOffset;

  void UpdatePosition() {
    transform.localRotation = Quaternion.identity;
    transform.localScale = new Vector3(10f/7f, 10f/7f, 1);
    transform.localPosition = new Vector3(model.position.x * transform.localScale.x, model.position.y * transform.localScale.y, 0);
  }

  void Awake() {
    randomOffset = Random.Range(0f, 100f);
  }

  void Start() {
    UpdatePosition();
  }

  public override void Rerender() {
    UpdatePosition();
  }

  public override bool Animating() {
    return false;
  }

  public override void Setup(Object2D _model, View _view) {
    base.Setup(_model, _view);

    if (model.plane.box.level.name == "Final Level") {
      view.model.levelCompletedEvent.AddListener(OnLevelCompletedEvent);
      UpdateFinalLevelColor();
      gameObject.layer = 0;
    }
  }

  void OnLevelCompletedEvent(Level level) {
    UpdateFinalLevelColor();
  }

  void UpdateFinalLevelColor() {
    maxOpactity = (Mathf.Max(((float) view.model.levels.Where(l => l.IsComplete()).Count() - 4), 0f) / ((float) view.model.levels.Count() - 5)) / 2f;
  }

  void Update() {
    if (model.plane.box.level.name == "Final Level" && !view.model.finalLevelAvailable) {
      GetComponent<SpriteRenderer>().color = new Color(
        1, 1, 1,
        Mathf.Sin((Time.time + randomOffset) / 4f) * maxOpactity
      );
    }
  }
}
