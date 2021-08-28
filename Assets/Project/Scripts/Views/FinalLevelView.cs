using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FinalLevelView : ViewModel<Level>, Clickable {
  public override void Setup(Level _model, View _view) {
    base.Setup(_model, _view);
    transform.localPosition = new Vector3(
      0,
      -62.5f,
      0
    );
  }
  
  public override bool Animating() {
    return false;
  }

  public override void Rerender() {}

  public void OnClick() {
    if (view.model.finalLevelAvailable) {
      view.controller.OnClickLevel(model);
    }
  }
}
