using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helpers2D
{
  public static Dictionary<Direction, Vector2Int> directionToVector2 = new Dictionary<Direction, Vector2Int>()
  {
      { Direction.Up, Vector2Int.up },
      { Direction.Down, Vector2Int.down },
      { Direction.Left, Vector2Int.left },
      { Direction.Right, Vector2Int.right }
  };

  public static Dictionary<Vector2Int, Direction> vector2ToDirection = new Dictionary<Vector2Int, Direction>()
  {
      { Vector2Int.up, Direction.Up },
      { Vector2Int.down, Direction.Down },
      { Vector2Int.left, Direction.Left },
      { Vector2Int.right, Direction.Right }
  };

  public static Dictionary<Direction, Direction> rotateDirectionRight = new Dictionary<Direction, Direction>()
  {
      { Direction.Up,    Direction.Right },
      { Direction.Right, Direction.Down },
      { Direction.Down,  Direction.Left },
      { Direction.Left,  Direction.Up }
  };

  public static Dictionary<Direction, Direction> oppositeDirection = new Dictionary<Direction, Direction>()
  {
      { Direction.Up,    Direction.Down },
      { Direction.Right, Direction.Left },
      { Direction.Down,  Direction.Up },
      { Direction.Left,  Direction.Right }
  };
}
