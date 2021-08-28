using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[System.Serializable]
public class MovePlayer2D : Move
{
  // initialize move
  public Model model;
  public Object2D player2d;
  public Direction direction;

  // information for undoing this move
  public Plane2D fromPlane;
  public Vector2Int fromPosition;
  public Object2D movedBox2d;
  public Plane2D boxFromPlane;
  public Vector2Int boxFromPosition;

  public MovePlayer2D(Model _model, Object2D _player2d, Direction _direction) {
    model = _model;
    player2d = _player2d;
    direction = _direction;
  }

  Plane2D FaceOnBox(Face face, Box3D box) {
    return box.faces.Find(plane2d => plane2d.face == face);
  }

  Box3D BoxAtLocation(Vector3Int position, List<Box3D> box3ds) {
    return box3ds.Find(box3d => box3d.position == position);
  }

  Object2D Box2DAtLocation(Plane2D plane2d, Vector2Int position) {
    return plane2d.boxes.Find(box => box.position == position);
  }

  int max2dUnit() {
    return model.units2d / 2;
  }

  bool CanMoveWithinPlane2D(Object2D object2d, Direction direction) {
    switch (direction)
    {
      case Direction.Up:
        return object2d.position.y != max2dUnit();
      case Direction.Down:
        return object2d.position.y != max2dUnit() * -1;
      case Direction.Left:
        return object2d.position.x != max2dUnit() * -1;
      case Direction.Right:
        return object2d.position.x != max2dUnit();
      default:
        throw new System.ComponentModel.InvalidEnumArgumentException();
    }
  }

  // Case A   Case B   Case C              
  //  🖾       🖾🖾     🖾|
  //  ‾|🖾     ‾‾‾‾     ‾‾‾

  Plane2D CaseAPlane2D(Plane2D plane2d, Direction direction) {
    Vector3Int position = plane2d.box.position;
    position += Helpers3D.faceToVector3[plane2d.face];
    position += Helpers3D.faceToVector3[Helpers3D.AdjacentFace(plane2d.face, direction)];
    Box3D box = BoxAtLocation(position, plane2d.box.level.box3ds);

    if (box == null) return null;

    Face face = Helpers3D.oppositeFace[Helpers3D.AdjacentFace(plane2d.face, direction)];

    return FaceOnBox(face, box);
  }

  Plane2D CaseBPlane2D(Plane2D plane2d, Direction direction) {
    Vector3Int position = plane2d.box.position;
    position += Helpers3D.faceToVector3[Helpers3D.AdjacentFace(plane2d.face, direction)];
    Box3D box = BoxAtLocation(position, plane2d.box.level.box3ds);

    if (box == null) return null;

    return FaceOnBox(plane2d.face, box);
  }

  Plane2D CaseCPlane2D(Plane2D plane2d, Direction direction) {
    return FaceOnBox(Helpers3D.AdjacentFace(plane2d.face, direction), plane2d.box);
  }

  Plane2D Plane2DToMoveOnto(Object2D object2d, Direction direction) {
    if (CanMoveWithinPlane2D(object2d, direction)) return object2d.plane;
    
    Plane2D caseA = CaseAPlane2D(object2d.plane, direction);
    if (caseA != null) return caseA;
    
    Plane2D caseB = CaseBPlane2D(object2d.plane, direction);
    if (caseB != null) return caseB;
    
    Plane2D caseC = CaseCPlane2D(object2d.plane, direction);
    if (caseC != null) return caseC;
    
    throw new ArgumentException("There is no plane to move onto");
  }

  // because -1 % 7 == -1, not 6
  int mod(int x, int m) {
    return (x%m + m)%m;
  }

  Vector2Int PositionToMoveOnto(Object2D object2d, Direction direction, Face toFace) {
    Vector2Int movedPosition = object2d.position + Helpers2D.directionToVector2[direction];
    return Helpers3D.PositionTranslation(
      object2d.plane.face,
      toFace,
      new Vector2Int(
        mod((movedPosition.x + max2dUnit()), model.units2d) - max2dUnit(),
        mod((movedPosition.y + max2dUnit()), model.units2d) - max2dUnit()
      )
    );
  }

  bool MoveBox2D(Object2D box2d, Direction direction) {
    Plane2D toPlane = Plane2DToMoveOnto(box2d, direction);
    Vector2Int toPosition = PositionToMoveOnto(box2d, direction, toPlane.face);

    if (
      model.IsWall2DAtLocation(toPlane, toPosition) ||
      model.IsBox2DAtLocation(toPlane, toPosition) ||
      model.IsPlayer2DAtLocation(toPlane, toPosition)
    ) {
      return false;
    }

    movedBox2d = box2d;
    boxFromPlane = box2d.plane;
    boxFromPosition = box2d.position;

    box2d.plane.boxes.Remove(box2d);
    toPlane.boxes.Add(box2d);
    box2d.plane = toPlane;
    box2d.position = toPosition;

    return true;
  }

  private void CheckLevelCompleted() {
    Level level = player2d.plane.box.level;
    if (
      level.box3ds.SelectMany(box3d => box3d.faces).All(
        face => face.goals.All(
          goal => face.boxes.Any(box => box.position == goal.position)
        )
      )
    ) {
      model.InvokeLevelCompleted(level);
    }
  }

  public bool Apply() {
    Plane2D toPlane = Plane2DToMoveOnto(player2d, direction);
    Vector2Int toPosition = PositionToMoveOnto(player2d, direction, toPlane.face);

    if (
      model.IsWall2DAtLocation(toPlane, toPosition) ||
      model.IsPlayer2DAtLocation(toPlane, toPosition)
    ) {
      return false;
    }

    Object2D box = Box2DAtLocation(toPlane, toPosition);
    if (box != null) {
      if (!MoveBox2D(box, Helpers3D.DirectionTranslation(
          player2d.plane.face,
          box.plane.face,
          direction
        ))) {
        return false;
      }
    }

    fromPlane = player2d.plane;
    fromPosition = player2d.position;
    player2d.plane.players.Remove(player2d);
    toPlane.players.Add(player2d);
    player2d.plane = toPlane;
    player2d.position = toPosition;

    // this line is NOT undone
    CheckLevelCompleted();

    return true;
  }

  public void Undo() {
    player2d.position = fromPosition;
    player2d.plane.players.Remove(player2d);
    player2d.plane = fromPlane;
    player2d.plane.players.Add(player2d);

    if (movedBox2d != null) {
      movedBox2d.position = boxFromPosition;
      movedBox2d.plane.boxes.Remove(movedBox2d);
      movedBox2d.plane = boxFromPlane;
      movedBox2d.plane.boxes.Add(movedBox2d);
    }
  }

  public void InvokeEvent() {
    model.InvokeUpdate(changed: new ModelEventPayload(
      _player2ds: new List<Object2D>(){ player2d },
      _box2ds: movedBox2d != null ?
        new List<Object2D>(){ movedBox2d } :
        null
    ));
  }
}
