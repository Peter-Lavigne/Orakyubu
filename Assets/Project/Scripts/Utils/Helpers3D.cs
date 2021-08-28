using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Helpers3D
{
  public static Dictionary<Face, Face> oppositeFace = new Dictionary<Face, Face>() {
    { Face.Front, Face.Back },
    { Face.Right, Face.Left },
    { Face.Back, Face.Front },
    { Face.Left, Face.Right },
    { Face.Top, Face.Bottom },
    { Face.Bottom, Face.Top }
  };

  public static Dictionary<Face, Vector3Int> faceToVector3 = new Dictionary<Face, Vector3Int>() {
      { Face.Front,  new Vector3Int(0, 0, -1) },
      { Face.Right,  new Vector3Int(1, 0, 0) },
      { Face.Back,   new Vector3Int(0, 0, 1) },
      { Face.Left,   new Vector3Int(-1, 0, 0) },
      { Face.Top,    new Vector3Int(0, 1, 0) },
      { Face.Bottom, new Vector3Int(0, -1, 0) }
  };

  public static Dictionary<Face, Face> nextFace = new Dictionary<Face, Face>() {
    { Face.Front,  Face.Top },
    { Face.Top,    Face.Right },
    { Face.Right,  Face.Front },
    { Face.Back,   Face.Left },
    { Face.Left,   Face.Bottom },
    { Face.Bottom, Face.Back }
  };

  public static Direction DirectionTranslation(Face from, Face to, Direction direction) {
    if (from == to) {
      return direction;
    } else if (nextFace[from] == to || oppositeFace[from] == to) {
      return Helpers2D.rotateDirectionRight[Helpers2D.rotateDirectionRight[Helpers2D.rotateDirectionRight[direction]]];
    } else if (nextFace[nextFace[from]] == to) {
      return Helpers2D.rotateDirectionRight[direction];
    } else
    {
      return Helpers2D.rotateDirectionRight[Helpers2D.rotateDirectionRight[direction]];
    }
  }

  public static Vector2Int PositionTranslation(Face from, Face to, Vector2Int position) {
    if (from == to) {
      return position;
    } else if (Helpers3D.nextFace[from] == to || oppositeFace[from] == to) {
      return new Vector2Int(-position.y, position.x);
    } else if (Helpers3D.nextFace[Helpers3D.nextFace[from]] == to) {
      return new Vector2Int(position.y, -position.x);
    } else {
      return new Vector2Int(-position.x, -position.y);
    }
  }

  public static Direction DirectionFromAToB(Face faceA, Vector2Int positionA, Face faceB, Vector2Int positionB) {
    Vector2Int poisitionBOnFaceA = PositionTranslation(faceB, faceA, positionB);
    Vector2Int difference = poisitionBOnFaceA - positionA;
    if (Math.Abs(difference.x) == 6 || Math.Abs(difference.y) == 6) difference = difference / -6;
    return Helpers2D.vector2ToDirection[difference];
  }

  public static Face AdjacentFace(Face face, Direction direction) {
    switch (direction) {
      case Direction.Up:
        return Helpers3D.nextFace[face];
      case Direction.Down:
        return Helpers3D.oppositeFace[Helpers3D.nextFace[face]];
      case Direction.Left:
        return Helpers3D.oppositeFace[Helpers3D.nextFace[Helpers3D.nextFace[face]]];
      case Direction.Right:
        return Helpers3D.nextFace[Helpers3D.nextFace[face]];
      default:
        throw new System.ComponentModel.InvalidEnumArgumentException();
    }
  }
}
