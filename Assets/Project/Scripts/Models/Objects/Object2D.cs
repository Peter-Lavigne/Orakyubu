using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Object2D
{
  public Vector2Int position;

  [System.NonSerialized]
  public Plane2D plane;

  public void SetPlane(Plane2D _plane)
  {
    plane = _plane;
  }
}
