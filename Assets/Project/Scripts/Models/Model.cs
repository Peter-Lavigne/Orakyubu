using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.IO;
using UnityEngine.SceneManagement;

public class Model : MonoBehaviour {
  public int units2d;

  public List<Level> levels;
  public ModelEvent updateEvent = new ModelEvent();
  public LevelCompletedEvent levelCompletedEvent = new LevelCompletedEvent();
  private Stack<Move> moves = new Stack<Move>();
  private Progress progress = new Progress();
  private string saveFilePath;
  [System.NonSerialized]
  public bool resetting = false;
  [System.NonSerialized]
  public bool finalLevelAvailable = false;

  public void InvokeUpdate(
    ModelEventPayload added = null,
    ModelEventPayload changed = null,
    ModelEventPayload removed = null
  ) {
    updateEvent.Invoke(
      added   != null ? added   : new ModelEventPayload(),
      changed != null ? changed : new ModelEventPayload(),
      removed != null ? removed : new ModelEventPayload()
    );
  }

  void UpdateFinalLevelAvailable() {
    finalLevelAvailable = levels.All(l => l.IsComplete() || l.name == "Final Level");
  }

  public void InvokeLevelCompleted(Level level) {
    if (!level.IsComplete()) {
      level.Complete();
      progress.completedLevels.Add(level.name);
      SaveProgress();
      UpdateFinalLevelAvailable();
    }
    levelCompletedEvent.Invoke(level);
  }

  void SaveProgress() {
    if (Globals.mode == Mode.main) {
      System.IO.File.WriteAllText(
        saveFilePath,
        JsonUtility.ToJson(progress, true)
      );
    }
  }

  void Awake() {
    saveFilePath = Application.persistentDataPath + "/Progress.json";
    Load();
  }

  void Start() {
    InvokeUpdate(added: new ModelEventPayload(levels));
  }

  private void ApplyMove(Move move) {
    if (move.Apply()) {
      moves.Push(move);
      move.InvokeEvent();
    }
  }

  public void MovePlayer2d(Object2D player2d, Direction direction) {
    ApplyMove(new MovePlayer2D(
      this,
      player2d,
      direction
    ));
  }

  public void PushBox3d(Box3D box3d, Face pushedFace) {
    ApplyMove(new MoveBox3D(
      this,
      box3d,
      pushedFace
    ));
  }

  public void Undo() {
    if (moves.Count == 0) return;
    Move move = moves.Pop();
    move.Undo();
    move.InvokeEvent();
  }

  public void Import(Level level) {
    ModelEventPayload removed = ModelEventPayload.FromCubes(levels[0].box3ds);
    levels[0].box3ds = level.box3ds;
    levels[0].name = level.name;
    SetupReferences(levels[0]);
    moves = new Stack<Move>();
    progress = new Progress();
    InvokeUpdate(
      added: ModelEventPayload.FromCubes(level.box3ds),
      removed: removed
    );
  }

  void Load() {
    try {
      progress = JsonUtility.FromJson<Progress>(
        System.IO.File.ReadAllText(saveFilePath)
      );
    } catch (FileNotFoundException e) {}

    string filePath = Globals.mode == Mode.main ? "StartingModel" : "Editor";
    levels = JsonUtility.FromJson<SavableModel>(
      Resources.Load<TextAsset>(filePath).text
    ).levels;

    if (Globals.mode == Mode.main) {
      foreach (string levelName in progress.completedLevels) {
        Level level = levels.FirstOrDefault(l => l.name == levelName);
        if (level != null) level.Complete();
      }
      UpdateFinalLevelAvailable();
    }

    foreach (Level level in levels) {
      SetupReferences(level);
    }
  }

  void SetupReferences(Level level) {
    foreach (Box3D box3d in level.box3ds) {
      box3d.SetLevel(level);
      foreach (Plane2D plane2d in box3d.faces) {
        plane2d.SetBox(box3d);
        foreach (Object2D player2d in plane2d.players) {
          player2d.SetPlane(plane2d);
        }
        foreach (Object2D box2d in plane2d.boxes) {
          box2d.SetPlane(plane2d);
        }
        foreach (Object2D wall2d in plane2d.walls) {
          wall2d.SetPlane(plane2d);
        }
        foreach (Object2D goal2d in plane2d.goals) {
          goal2d.SetPlane(plane2d);
        }
      }
    }
  }

  public void Reset() {
    resetting = true;
    if (Globals.mode == Mode.editor) {
      levels[0].Uncomplete();
    }
    while (moves.Count > 0) Undo();
    resetting = false;
  }

  private Object2D AddObject2d(Plane2D plane, List<Object2D> planeGroup, Vector2Int position) {
    Object2D obj = new Object2D();
    obj.position = position;
    planeGroup.Add(obj);
    obj.SetPlane(plane);
    return obj;
  }

  public bool IsPlayer2DAtLocation(Plane2D plane2d, Vector2Int position) {
    return plane2d.players.Any(player => player.position == position);
  }
  
  public bool IsWall2DAtLocation(Plane2D plane2d, Vector2Int position) {
    return plane2d.walls.Any(wall => wall.position == position);
  }

  public bool IsBox2DAtLocation(Plane2D plane2d, Vector2Int position) {
    return plane2d.boxes.Any(box => box.position == position);
  }

  public bool IsTargetAtLocation(Plane2D plane2d, Vector2Int position) {
    return plane2d.goals.Any(box => box.position == position);
  }

  public void AddPlayer2d(Plane2D plane, Vector2Int position) {
    if (
      IsWall2DAtLocation(plane, position) ||
      IsBox2DAtLocation(plane, position) ||
      IsPlayer2DAtLocation(plane, position)
    ) return;
    Object2D player2d = AddObject2d(plane, plane.players, position);
    InvokeUpdate(added: new ModelEventPayload(
      _player2ds: new List<Object2D>(){ player2d }
    ));
  }

  public void AddBox2d(Plane2D plane, Vector2Int position) {
    if (
      IsWall2DAtLocation(plane, position) ||
      IsBox2DAtLocation(plane, position) ||
      IsPlayer2DAtLocation(plane, position)
    ) return;
    Object2D box2d = AddObject2d(plane, plane.boxes, position);
    InvokeUpdate(added: new ModelEventPayload(
      _box2ds: new List<Object2D>(){ box2d }
    ));
  }

  public void AddTarget2d(Plane2D plane, Vector2Int position) {
    if (
      IsWall2DAtLocation(plane, position) ||
      IsTargetAtLocation(plane, position)
    ) return;
    Object2D target = AddObject2d(plane, plane.goals, position);
    InvokeUpdate(added: new ModelEventPayload(
      _targets: new List<Object2D>(){ target }
    ));
  }

  public void AddWall2d(Plane2D plane, Vector2Int position) {
    if (
      IsWall2DAtLocation(plane, position) ||
      IsBox2DAtLocation(plane, position) ||
      IsPlayer2DAtLocation(plane, position) ||
      IsTargetAtLocation(plane, position)
    ) return;
    Object2D wall2d = AddObject2d(plane, plane.walls, position);
    InvokeUpdate(added: new ModelEventPayload(
      _walls: new List<Object2D>(){ wall2d }
    ));
  }

  public bool IsBox3dAtLocation(Vector3Int position, List<Box3D> box3ds) {
    return box3ds.Any(box3d => box3d.position == position);
  }

  public void AddBox3d() {
    if (levels[0].box3ds.Count() == 7 * 7 * 6) return;
    Vector3Int position = new Vector3Int(0, -1, 0);
    int[] order = new int[] { 0, 1, -1, 2, -2, 3, -3 };
    for (int y = 0; y < 6; y++) {
      for (int z = 0; z < order.Length; z++) {
        for (int x = 0; x < order.Length; x++) {
          if (!IsBox3dAtLocation(
            new Vector3Int(order[x], y, order[z]),
            levels[0].box3ds
          )) {
            position = new Vector3Int(order[x], y, order[z]);
            break;
          }
        }
        if (position.y != -1) break;
      }
      if (position.y != -1) break;
    }

    Box3D box3d = new Box3D();
    box3d.position = position;
    levels[0].box3ds.Add(box3d);
    box3d.SetLevel(levels[0]);
    box3d.faces = new List<Plane2D>(){
      new Plane2D(),
      new Plane2D(),
      new Plane2D(),
      new Plane2D(),
      new Plane2D(),
      new Plane2D()
    };
    box3d.faces[0].face = Face.Front;
    box3d.faces[1].face = Face.Right;
    box3d.faces[2].face = Face.Back;
    box3d.faces[3].face = Face.Left;
    box3d.faces[4].face = Face.Top;
    box3d.faces[5].face = Face.Bottom;
    foreach (Plane2D plane2d in box3d.faces) {
      plane2d.players = new List<Object2D>();
      plane2d.boxes = new List<Object2D>();
      plane2d.walls = new List<Object2D>();
      plane2d.goals = new List<Object2D>();
      plane2d.SetBox(box3d);
    }
    InvokeUpdate(added: new ModelEventPayload(
      _box3ds: new List<Box3D>(){ box3d },
      _planes: box3d.faces
    ));
  }

  public void RemoveFromPosition(Plane2D plane, Vector2Int position) {
    List<Object2D> player2ds = plane.players.Where(player2d => player2d.position == position).ToList();
    foreach (Object2D object2d in player2ds) {
      plane.players.Remove(object2d);
    }
    List<Object2D> box2ds = plane.boxes.Where(box2d => box2d.position == position).ToList();
    foreach (Object2D object2d in box2ds) {
      plane.boxes.Remove(object2d);
    }
    List<Object2D> targets = plane.goals.Where(target => target.position == position).ToList();
    foreach (Object2D object2d in targets) {
      plane.goals.Remove(object2d);
    }
    List<Object2D> walls = plane.walls.Where(wall => wall.position == position).ToList();
    foreach (Object2D object2d in walls) {
      plane.walls.Remove(object2d);
    }
    InvokeUpdate(removed: new ModelEventPayload(
      _player2ds: player2ds,
      _box2ds: box2ds,
      _targets: targets,
      _walls: walls
    ));
  }

  public void RemoveCube(Box3D cube) {
    if (cube.level.box3ds.Count() == 1) return;
    cube.level.box3ds.Remove(cube);
    InvokeUpdate(removed: ModelEventPayload.FromCubes(new List<Box3D>(){cube}));
  }

  public bool PositionInbounds(Vector3Int position, bool finalLevel) {
    if (finalLevel) {
      return (
        position.x > -3 &&
        position.x < 3 &&
        position.z > -3 &&
        position.z < 3
      );
    }
    return (
      position.x > -4 &&
      position.x < 4 &&
      position.z > -4 &&
      position.z < 4 &&
      position.y >= 0 &&
      position.y < 6
    );
  }

  public void MoveCubeEditMode(Box3D cube, Vector3Int position) {
    if (!PositionInbounds(position, false)) return;
    if (IsBox3dAtLocation(position, cube.level.box3ds)) return;
    cube.position = position;
    InvokeUpdate(changed: new ModelEventPayload(
      _box3ds: new List<Box3D>(){ cube }
    ));
  }
}
