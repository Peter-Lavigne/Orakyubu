using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class MoveBox3D : Move
{
  public Model model;
  public Box3D box3d;
  public Face pushedFace;
  public Vector3Int fromPosition;

  public MoveBox3D(Model _model, Box3D _box3d, Face _pushedFace) {
    model = _model;
    box3d = _box3d;
    pushedFace = _pushedFace;
  }

  public bool Apply() {
    fromPosition = box3d.position;
    Vector3Int toPosition = fromPosition + Helpers3D.faceToVector3[Helpers3D.oppositeFace[pushedFace]];

    if (model.IsBox3dAtLocation(toPosition, box3d.level.box3ds)) return false;

    if (!model.PositionInbounds(toPosition, box3d.level.name == "Final Level")) return false;
  
    box3d.position = toPosition;
    return true;
  }

  public void Undo() {
    box3d.position = fromPosition;
  }

  public void InvokeEvent() {
    model.InvokeUpdate(changed: new ModelEventPayload(
      _box3ds: new List<Box3D>(){ box3d }
    ));
  }
}
