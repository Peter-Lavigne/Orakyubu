using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Box3D
{
  public Vector3Int position;
  public List<Plane2D> faces;

  [System.NonSerialized]
  public Level level;

  public void SetLevel(Level _level)
  {
    level = _level;
  }
}
