using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box3DView : ViewModel<Box3D> {
  public GameObject controller;
  public Animator positionAnimator;
  public GameObject selection;
  public GameObject hover;

  private Vector3 moveFrom;
  private Vector3 moveTo = new Vector3(1000f, 0, 0);

  private bool oneTimeSlowDown = false;

  public override void Rerender() {
    if (model.level.name == "Final Level" && !view.model.finalLevelAvailable) return;

    if (model.position != moveTo) {
      if (moveTo.x > 999f) {
        moveFrom = transform.localPosition;
        oneTimeSlowDown = true;
        positionAnimator.totalSeconds = 2.5f;
      } else {
        moveFrom = moveTo;
      }
      moveTo = model.position;
      positionAnimator.Start();
      if (!view.model.resetting) {
        view.cubeMoveSound.GetComponent<AudioSource>().pitch = (Random.Range(0.38f, 0.42f));
        view.cubeMoveSound.GetComponent<AudioSource>().PlayOneShot(view.cubeMoveSound.GetComponent<AudioSource>().clip, 1f);
      }
    }

    if (view.controller.GetSelectedBox3d() == model) {
      selection.SetActive(true);
      hover.SetActive(false);
    } else {
      selection.SetActive(false);
      if (view.mainInput.GetHoveredBox3d() == model && view.controller.GetCurrentLevel().box3ds.Count > 1) {
        hover.SetActive(true);
      } else {
        hover.SetActive(false);
      }
    }
  }

  public override void Setup(Box3D _model, View _view) {
    base.Setup(_model, _view);
    gameObject.transform.localPosition = model.position;
    gameObject.transform.localScale = new Vector3(1, 1, 1);
    gameObject.transform.localRotation = Quaternion.identity;
    if (model.level.name != "Final Level" || view.model.finalLevelAvailable) {
      moveFrom = model.position;
      moveTo = model.position;
    }
  }
  
  public override bool Animating() {
    return positionAnimator.Animating();
  }

  void Update() {
    if (!Animating()) return;

    positionAnimator.Update(
      t => {
        gameObject.transform.localPosition = Vector3.Lerp(
          moveFrom,
          moveTo,
          t
        );
      },
      () => {
        gameObject.transform.localPosition = moveTo;
        if (oneTimeSlowDown) {
          positionAnimator.totalSeconds = 0.2f;
        }
      }
    );
  }
}
