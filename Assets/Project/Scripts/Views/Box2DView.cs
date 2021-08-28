using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Box2DView : MovableObject2DView
{
  public Color defaultColor;
  public Color completedColor;

  public override void Rerender()
  {
    base.Rerender();
    if (model.plane.box.level.name != "Final Level" || view.model.finalLevelAvailable) {
      if (model.plane.goals.Any(goal => goal.position == model.position)) {
        spriteFrom.GetComponent<SpriteRenderer>().color = completedColor;
        spriteTo.GetComponent<SpriteRenderer>().color = completedColor;
      } else {
        spriteFrom.GetComponent<SpriteRenderer>().color = defaultColor;
        spriteTo.GetComponent<SpriteRenderer>().color = defaultColor;
      }
    }
  }
}
