using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Plane2D
{
  public Face face;
  public List<Object2D> players;
  public List<Object2D> boxes;
  public List<Object2D> goals;
  public List<Object2D> walls;

  [System.NonSerialized]
  public Box3D box;

  public void SetBox(Box3D _box)
  {
    box = _box;
  }
}
