using System.Collections;
using System.Collections.Generic;
using System.Linq;

class ControllerHelpers {
  public static List<List<string>> levelOrder = new List<List<string>>(){
    new List<string>() {
      "Intro to 2D"
    },
    new List<string>() {
      "Intro to 3D"
    },
    new List<string>() {
      "Two Boxes"
    },
    new List<string>() {
      "Two Cubes"
    },
    new List<string>() {
      "Across the Top",
      "To the Back"
    },
    new List<string>() {
      "Intro to Escaping"
    },
    new List<string>() {
      "Bring a Box Around",
      "Swap Them Primer"
    },
    new List<string>() {
      "Go Around",
      "Save a Box"
    },
    new List<string>() {
      "Don't Push Too Far",
      "Intro to Corners",
      "Full Corner"
    },
    new List<string>() {
      "Diagonal"
    },
    new List<string>() {
      "Turn Around",
      "Beginner Diagonal Puzzle"
    },
    new List<string>() {
      "Less Obvious Houdini",
      "Swap Them with Three Faces"
    },
    new List<string>() {
      "Rook Their Way",
      "Rook My Way"
    },
    new List<string>() {
      "Floating Houdini"
    },
    new List<string>() {
      "Floating Intersection",
      "Three Swap"
    }
  };

  public static int LevelIndexX(string levelName) {
    return ControllerHelpers.levelOrder.IndexOf(
      ControllerHelpers
        .levelOrder
        .Single(list => list.Contains(levelName))
    );
  }

  public static int LevelIndexY(string levelName) {
    return ControllerHelpers
      .levelOrder
      .Single(list => list.Contains(levelName))
      .IndexOf(levelName);
  }

  public static int LevelNumber(string levelName) {
    if (levelName == "Final Level") return 25;
    return ControllerHelpers.levelOrder.SelectMany(l => l).ToList().IndexOf(levelName) + 1;
  }

  public static int LevelCountInColumn(string levelName) {
    return ControllerHelpers
      .levelOrder
      .Single(list => list.Contains(levelName))
      .Count();
  }

  public static List<string> PreviousLevels(string levelName) {
    if (levelName == "Intro to 2D") return levelOrder[levelOrder.Count() - 1];
    if (levelName == "Custom Level") return new List<string>();
    

    int yIndex = LevelIndexY(levelName);
    List<string> previousColumn = levelOrder[LevelIndexX(levelName) - 1];
    int levelCountInColumn = LevelCountInColumn(levelName);
    int levelCountInPreviousColumn = previousColumn.Count();
    
    List<string> previousLevels = new List<string>();

    if (
      yIndex == 0 ||
      levelCountInPreviousColumn == 1 ||
      (yIndex == 1 && levelCountInColumn == 3 && levelCountInPreviousColumn == 2)
    ) {
      previousLevels.Add(previousColumn[0]);
    }
    if (
      levelCountInPreviousColumn > 1 && (
        yIndex == 1 ||
        levelCountInColumn < levelCountInPreviousColumn ||
        (levelCountInColumn == 3 && levelCountInPreviousColumn == 2 && yIndex == 2)
      )
    ) {
      previousLevels.Add(previousColumn[1]);
    }
    if (
      levelCountInPreviousColumn == 3 &&
      yIndex == levelCountInColumn - 1
    ) {
      previousLevels.Add(previousColumn[2]);
    }

    return previousLevels;
  }

  public static IEnumerable<string> NextLevels(string levelName) {
    IEnumerable<string> allLevels = levelOrder.SelectMany(l => l);
    return allLevels.Where(level => PreviousLevels(level).Contains(levelName));
  }

  public static Dictionary<Face, Face> perceivedRightFace = new Dictionary<Face, Face>() {
    { Face.Front, Face.Right },
    { Face.Right, Face.Back },
    { Face.Back, Face.Left },
    { Face.Left, Face.Front }
  };

  public static Direction PerceivedFaceDirectionToTrueDirection(Direction direction, Face onFace, Face facingFace) {
    switch (onFace) {
      case Face.Front:
        return direction;
      case Face.Right:
        return Helpers2D.rotateDirectionRight[direction];
      case Face.Back:
        return Helpers2D.rotateDirectionRight[Helpers2D.rotateDirectionRight[Helpers2D.rotateDirectionRight[direction]]];
      case Face.Left:
        return Helpers2D.rotateDirectionRight[Helpers2D.rotateDirectionRight[direction]];
      case Face.Top:
        switch (facingFace) {
          case Face.Front:
            return Helpers2D.rotateDirectionRight[Helpers2D.rotateDirectionRight[Helpers2D.rotateDirectionRight[direction]]];
          case Face.Right:
            return Helpers2D.rotateDirectionRight[Helpers2D.rotateDirectionRight[direction]];
          case Face.Back:
            return Helpers2D.rotateDirectionRight[direction];
          case Face.Left:
            return direction;
          default:
            throw new System.ComponentModel.InvalidEnumArgumentException();
        }
      case Face.Bottom:
        switch (facingFace) {
          case Face.Front:
            return Helpers2D.rotateDirectionRight[Helpers2D.rotateDirectionRight[direction]];
          case Face.Right:
            return Helpers2D.rotateDirectionRight[Helpers2D.rotateDirectionRight[Helpers2D.rotateDirectionRight[direction]]];
          case Face.Back:
            return direction;
          case Face.Left:
            return Helpers2D.rotateDirectionRight[direction];
          default:
            throw new System.ComponentModel.InvalidEnumArgumentException();
        }
      default:
        throw new System.ComponentModel.InvalidEnumArgumentException();
    }
  }
}
