using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Wall2DView : Object2DView {
  public Color defaultColor;
  public Color completedColor;

  public void UpdateColor() {
    GetComponent<SpriteRenderer>().color = model.plane.box.level.IsComplete() ?
      completedColor :
      defaultColor;
  }

  public override void Rerender() {
    base.Rerender();

    if (view.controller.GetCurrentLevel() == model.plane.box.level || view.model.finalLevelAvailable) {
      UpdateColor();
    }
  }

  public override void Setup(Object2D _model, View _view) {
    base.Setup(_model, _view);
    UpdateColor();
  }
}
