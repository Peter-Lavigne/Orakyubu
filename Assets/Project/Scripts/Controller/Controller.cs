using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public enum EditObject {
  Player2D,
  Box2D,
  Goal2D,
  Wall2D,
  Remove,
  RemoveCube,
  MoveCubeUp,
  MoveCubeDown,
  MoveCubeAway
};
public enum ControlMode { Puzzle, Edit, LevelSelect, Credits }

public class Controller : MonoBehaviour {
  public Model model;
  public Settings settings;

  private Object2D selectedPlayer2d = null;
  private Box3D selectedBox = null;
  private bool editing = false;
  private EditObject currentlyEditing = EditObject.Player2D;
  private Level currentLevel;
  private ControlMode controlMode = ControlMode.Puzzle;

  public CurrentLevelChangedEvent currentLevelChangedEvent = new CurrentLevelChangedEvent();
  public ControlModeChangedEvent controlModeChangedEvent = new ControlModeChangedEvent();

  void Awake() {
    model.updateEvent.AddListener(HandleModelEvent);
    model.levelCompletedEvent.AddListener(HandleLevelCompletedEvent);
  }

  void HandleLevelCompletedEvent(Level level) {
    if (level.name == "Final Level") {
      SetControlMode(ControlMode.Credits);
      selectedBox = null;
      selectedPlayer2d = null;
    }
  }

  void HandleModelEvent(
    ModelEventPayload added,
    ModelEventPayload changed,
    ModelEventPayload removed
  ) {
    if (removed.player2ds.Contains(selectedPlayer2d)) selectedPlayer2d = null;
    if (removed.box3ds.Contains(selectedBox)) selectedBox = null;
    if (removed.levels.Contains(currentLevel)) {
      currentLevel = added.levels.Single(level => level.name == currentLevel.name);
      currentLevelChangedEvent.Invoke(currentLevel);
    }
    if (controlMode == ControlMode.Edit && added.levels.Count() > 0) {
      currentLevel = added.levels.First();
      currentLevelChangedEvent.Invoke(currentLevel);
    }
  }

  public void Reset() { model.Reset(); }
  public void Undo() { model.Undo(); }

  public Box3D GetSelectedBox3d() { return selectedBox; }
  public Level GetCurrentLevel() { return currentLevel; }
  public Object2D GetSelectedPlayer2d() { return selectedPlayer2d; }
  public void SetEditObject(EditObject _currentlyEditing) { currentlyEditing = _currentlyEditing; }
  public ControlMode GetControlMode() { return controlMode; }

  public void SetControlMode(ControlMode _controlMode) {
    if (_controlMode == controlMode) return;
    controlMode = _controlMode;
    if (controlMode == ControlMode.LevelSelect) {
      selectedBox = null;
      selectedPlayer2d = null;
      Reset();
    }
    if (controlMode == ControlMode.Edit) {
      selectedBox = null;
      selectedPlayer2d = null;
      Reset();
    }
    controlModeChangedEvent.Invoke(controlMode);
  }

  [DllImport("__Internal")]
  private static extern void StartLevelEvent(int level);

  public void OnClickLevel(Level level) {
    currentLevel = level;

    #if UNITY_WEBGL
    StartLevelEvent(ControllerHelpers.LevelNumber(level.name));
    #endif

    currentLevelChangedEvent.Invoke(currentLevel);
    SetControlMode(ControlMode.Puzzle);
  }

  void Start() {
    string firstLevelName = Globals.mode == Mode.main ? ControllerHelpers.levelOrder.First().First() : "Custom Level";
    Level firstLevel = model.levels.Find(level => level.name == firstLevelName);
    if (!firstLevel.IsComplete()) {
      currentLevel = firstLevel;
      currentLevelChangedEvent.Invoke(currentLevel);
      SetControlMode(Globals.mode == Mode.main ? ControlMode.Puzzle : ControlMode.Edit);
    } else {
      SetControlMode(ControlMode.LevelSelect);
    }
  }

  public void OnMouseDownPosition(Plane2D plane, Vector2Int position) {
    if (controlMode != ControlMode.Edit) return;

    switch (currentlyEditing) {
      case EditObject.Player2D:
        model.AddPlayer2d(plane, position);
        break;
      case EditObject.Box2D:
        model.AddBox2d(plane, position);
        break;
      case EditObject.Goal2D:
        model.AddTarget2d(plane, position);
        break;
      case EditObject.Wall2D:
        model.AddWall2d(plane, position);
        break;
      case EditObject.Remove:
        model.RemoveFromPosition(plane, position);
        break;
      default:
        break;
    }
  }

  public void OnClickPosition(Plane2D plane, Vector2Int position, Face facingFace, bool animating) {
    if (!currentLevel.box3ds.SelectMany(box3d => box3d.faces).Contains(plane)) return;

    if (controlMode == ControlMode.Edit) {
      switch (currentlyEditing) {
        case EditObject.RemoveCube:
          model.RemoveCube(plane.box);
          break;
        case EditObject.MoveCubeUp:
          model.MoveCubeEditMode(plane.box, plane.box.position + Vector3Int.up);
          break;
        case EditObject.MoveCubeDown:
          model.MoveCubeEditMode(plane.box, plane.box.position + Vector3Int.down);
          break;
        case EditObject.MoveCubeAway:
          model.MoveCubeEditMode(plane.box, plane.box.position + Helpers3D.faceToVector3[Helpers3D.oppositeFace[facingFace]]);
          break;
        default:
          break;
      }
    } else {
      Object2D player = plane.players
        .FirstOrDefault(player2d => player2d.position == position);
      if (player == null) {
        if (currentLevel.box3ds.Count() > 1) {
          selectedPlayer2d = null;
          selectedBox = plane.box;
        }
      } else if (!animating) {
        selectedBox = null;
        selectedPlayer2d = player;
      }
    }
  }

  private void MovePlayer2d(Direction direction, Face facingFace) {
    model.MovePlayer2d(
      GetSelectedPlayer2d(),
      ControllerHelpers.PerceivedFaceDirectionToTrueDirection(
        direction,
        GetSelectedPlayer2d().plane.face,
        facingFace
      )
    );
  }

  public void Move(Direction direction, Face facingFace) {
    if (selectedPlayer2d != null) {
      MovePlayer2d(direction, facingFace);
    } else if (selectedBox != null) {
      Face rightFace = ControllerHelpers.perceivedRightFace[facingFace];
      switch (direction) {
        case Direction.Up:
          model.PushBox3d(selectedBox, facingFace);
          break;
        case Direction.Down:
          model.PushBox3d(selectedBox, Helpers3D.oppositeFace[facingFace]);
          break;
        case Direction.Right:
          model.PushBox3d(selectedBox, Helpers3D.oppositeFace[rightFace]);
          break;
        case Direction.Left:
          model.PushBox3d(selectedBox, rightFace);
          break;
        default:
          break;
      }
    }
  }

  public void CreateBox3d() {
    if (controlMode != ControlMode.Edit) return;
    model.AddBox3d();
  }
}
