using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MovableObject2DView : ViewModel<Object2D>
{
  private float maxOpactity = 0f;
  private float randomOffset;

  public GameObject spriteFrom;
  public GameObject spriteTo;
  public Animator moveAnimator;

  private GameObject moveFromPlane;
  private Plane2D moveFromPlane2d;
  private Vector2Int moveFrom1;
  private Vector2Int moveFrom2;

  private GameObject moveToPlane;
  private Plane2D moveToPlane2d;
  private Vector2Int moveTo1;
  private Vector2Int moveTo2;

  void OnDestroy() {
    Destroy(spriteFrom);
    Destroy(spriteTo);
  }

  void Awake() {
    randomOffset = Random.Range(0f, 100f);
  }

  void UpdatePosition()
  {
    transform.localRotation = Quaternion.identity;
    transform.localScale = new Vector3(10f/7f, 10f/7f, 1);
    transform.localPosition = new Vector3(model.position.x * transform.localScale.x, model.position.y * transform.localScale.y, 0);
  }

  public override void Setup(Object2D _model, View _view)
  {
    base.Setup(_model, _view);
    moveToPlane = view.GetViewByModel(model.plane);
    moveToPlane2d = model.plane;

    moveTo2 = model.position;
    UpdatePosition();

    spriteFrom.transform.SetParent(moveToPlane.transform, true);
    spriteTo.transform.SetParent(moveToPlane.transform, true);

    if (model.plane.box.level.name == "Final Level") {
      view.model.levelCompletedEvent.AddListener(OnLevelCompletedEvent);
      UpdateFinalLevelColor();
      spriteFrom.layer = 0;
      spriteTo.layer = 0;
    }
  }

  void OnLevelCompletedEvent(Level level) {
    UpdateFinalLevelColor();
  }

  void UpdateFinalLevelColor() {
    maxOpactity = (Mathf.Max(((float) view.model.levels.Where(l => l.IsComplete()).Count() - 4), 0f) / ((float) view.model.levels.Count() - 5)) / 2f;
  }

  public override void Rerender()
  {
    if (model.position != moveTo2 || model.plane != moveToPlane2d)
    {
      moveFromPlane = moveToPlane;
      moveFromPlane2d = moveToPlane2d;

      spriteFrom.transform.SetParent(moveFromPlane.transform, false);

      moveToPlane = view.GetViewByModel(model.plane);
      moveToPlane2d = model.plane;

      spriteTo.transform.SetParent(moveToPlane.transform, false);
      spriteTo.transform.localRotation = Quaternion.identity;

      moveFrom1 = moveTo2;
      Direction lastDirectionMoved = Helpers3D.DirectionFromAToB(
        moveFromPlane2d.face,
        moveFrom1,
        moveToPlane2d.face,
        model.position
      );
      moveFrom2 = moveFrom1 + Helpers2D.directionToVector2[lastDirectionMoved];

      moveTo1 = model.position - Helpers2D.directionToVector2[Helpers3D.DirectionTranslation(
        moveFromPlane2d.face,
        model.plane.face,
        lastDirectionMoved
      )];
      moveTo2 = model.position;

      UpdateSpritePositions(0);
      moveAnimator.Start();
      spriteTo.SetActive(true);

      if (!view.model.resetting) {
        view.playerMoveSound.GetComponent<AudioSource>().pitch = (Random.Range(0.6f, 0.65f));
        view.playerMoveSound.GetComponent<AudioSource>().PlayOneShot(view.playerMoveSound.GetComponent<AudioSource>().clip, 1f);
      }

      UpdatePosition();
    }
  }

  public override bool Animating() {
    return moveAnimator.Animating();
  }

  void UpdateSpritePositions(float t) {
    spriteFrom.transform.localPosition = Vector3.Lerp(
      new Vector3(moveFrom1.x * transform.localScale.x, moveFrom1.y * transform.localScale.y, 0f),
      new Vector3(moveFrom2.x * transform.localScale.x, moveFrom2.y * transform.localScale.y, 0f),
      t
    );
    spriteTo.transform.localPosition = Vector3.Lerp(
      new Vector3(moveTo1.x * transform.localScale.x, moveTo1.y * transform.localScale.y, 0f),
      new Vector3(moveTo2.x * transform.localScale.x, moveTo2.y * transform.localScale.y, 0f),
      t
    );
  }

  void UpdateMoveAnimator() {
    moveAnimator.Update(
      t => {
        UpdateSpritePositions(t);
      },
      () => {
        spriteFrom.transform.SetParent(spriteTo.transform.parent.transform, false);
        spriteFrom.transform.position = spriteTo.transform.position;
        spriteFrom.transform.localRotation = Quaternion.identity;
        spriteFrom.transform.localPosition = new Vector3(
          spriteFrom.transform.localPosition.x,
          spriteFrom.transform.localPosition.y,
          0f
        );
        spriteTo.SetActive(false);
      }
    );
  }

  public void Update()
  {
    if (model.plane.box.level.name == "Final Level" && !view.model.finalLevelAvailable) {
      Color fadeColor = new Color(
        1, 1, 1,
        Mathf.Sin((Time.time + randomOffset) / 4f) * maxOpactity
      );
      spriteFrom.GetComponent<SpriteRenderer>().color = fadeColor;
      spriteTo.GetComponent<SpriteRenderer>().color = fadeColor;
    }

    if (!Animating()) {
      return;
    }

    UpdateMoveAnimator();
  }
}
