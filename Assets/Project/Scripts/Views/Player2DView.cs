using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class Player2DView : MovableObject2DView
{
  public Color hoveredColor;
  public Color selectedColor;

  public override void Setup(Object2D _model, View _view)
  {
    base.Setup(_model, _view);
  }

  public void Update()
  {
    base.Update();

    if (model.plane.box.level.name != "Final Level" || view.model.finalLevelAvailable) {
      if (view.controller.GetSelectedPlayer2d() == model) {
        spriteFrom.GetComponent<SpriteRenderer>().color = selectedColor;
        spriteTo.GetComponent<SpriteRenderer>().color = selectedColor;
      } else if (view.mainInput.GetHoveredPlayer2d() == model) {
        spriteFrom.GetComponent<SpriteRenderer>().color = hoveredColor;
        spriteTo.GetComponent<SpriteRenderer>().color = hoveredColor;
      } else {
        spriteFrom.GetComponent<SpriteRenderer>().color = Color.white;
        spriteTo.GetComponent<SpriteRenderer>().color = Color.white;
      }
    }
  }
}
