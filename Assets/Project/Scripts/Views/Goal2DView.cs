using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Goal2DView : Object2DView
{
  public Color defaultColor;
  public Color completedColor;

  public override void Setup(Object2D _model, View _view) {
    base.Setup(_model, _view);

    if (model.plane.boxes.Any(box => box.position == model.position)) {
      GetComponent<SpriteRenderer>().color = completedColor;
    }
  }

  public override void Rerender()
  {
    base.Rerender();

    if (view.controller.GetCurrentLevel() == model.plane.box.level || view.model.finalLevelAvailable) {
      if (model.plane.boxes.Any(box => box.position == model.position)) {
        if (
          GetComponent<SpriteRenderer>().color != completedColor
        ) {
          if (!view.model.resetting) {
            view.targetFilledSound.GetComponent<AudioSource>().PlayOneShot(view.targetFilledSound.GetComponent<AudioSource>().clip, 1f);
          }
        }
        GetComponent<SpriteRenderer>().color = completedColor;
      } else {
        GetComponent<SpriteRenderer>().color = defaultColor;
      }
    }
  }
}
