using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Plane2DView : ViewModel<Plane2D>
{
  public override void Setup(Plane2D _model, View _view)
  {
    base.Setup(_model, _view);
    UpdatePosition();
    gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
  }

  private Dictionary<Face, Vector3> faceTransforms = new Dictionary<Face, Vector3>()
  {
      { Face.Front, Vector3.back * 0.5f },
      { Face.Right, Vector3.right * 0.5f },
      { Face.Back, Vector3.forward * 0.5f },
      { Face.Left, Vector3.left * 0.5f },
      { Face.Top, Vector3.up * 0.5f },
      { Face.Bottom, Vector3.down * 0.5f }
  };

  private Dictionary<Face, Quaternion> faceRotations = new Dictionary<Face, Quaternion>()
  {
      { Face.Front, Quaternion.Euler(0f, 0f, 0f) },
      { Face.Right, Quaternion.Euler(0f, -90f, 90f) },
      { Face.Back, Quaternion.Euler(0f, 180f, -90f) },
      { Face.Left, Quaternion.Euler(0f, 90f, 180f) },
      { Face.Top, Quaternion.Euler(90f, 90f, 0f) },
      { Face.Bottom, Quaternion.Euler(-90f, 180f, 0f) }
  };

  void UpdatePosition()
  {
    transform.localPosition = faceTransforms[model.face] * 1.0003f;
    transform.localRotation = faceRotations[model.face];
  }

  public override void Rerender()
  {
    return;
  }

  public override bool Animating()
  {
    return false;
  }

  Vector2Int ClosestPositionToPoint(Vector3 point) {
    Vector2Int closest = new Vector2Int(
      int.MaxValue,
      int.MaxValue
    );
    float offsetMultiplier = (model.box.level.name == "Final Level" ? 25f : 1f) / (float) view.model.units2d;
    float closestDistance = float.MaxValue;
    for (int x = -3; x <= 3; x++) {
      for (int y = -3; y <= 3; y++) {
        Vector3 offset = new Vector3(
          (float) x * offsetMultiplier,
          (float) y * offsetMultiplier,
          0
        );
        float distance = Vector3.Distance(
          point,
          transform.position + (transform.rotation * offset)
        );
        if (distance < closestDistance) {
          closestDistance = distance;
          closest = new Vector2Int(x, y);
        }
      }
    }
    if (closest.x == int.MaxValue) {
      throw new InvalidOperationException("Unable to determine closest point");
    }
    return closest;
  }

  public void OnClick(Vector3 point) {
    view.controller.OnClickPosition(model, ClosestPositionToPoint(point), view.mainInput.mainCamera.FacingFace(), view.Animating());
  }

  public void OnMouseDown(Vector3 point) {
    view.controller.OnMouseDownPosition(model, ClosestPositionToPoint(point));
  }

  public void OnHover(Vector3 point) {
    view.mainInput.OnHoverPosition(model, ClosestPositionToPoint(point));
  }
}
