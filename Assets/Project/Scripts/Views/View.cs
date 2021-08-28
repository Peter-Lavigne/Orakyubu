using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class View : MonoBehaviour, Renderable
{
  public Model model;
  public MainInput mainInput;
  public Controller controller;
  public GameObject levelPrefab;
  public GameObject finalLevelPrefab;
  public GameObject box3dPrefab;
  public GameObject plane2dPrefab;
  public GameObject player2dPrefab;
  public GameObject box2dPrefab;
  public GameObject goal2dPrefab;
  public GameObject wall2dPrefab;
  public Camera camera;
  public GameObject progressionLinePrefab;
  public GameObject levelCompleteSound;
  public GameObject playerMoveSound;
  public GameObject cubeMoveSound;
  public GameObject targetFilledSound;
  public LevelCompletedPause levelCompletedPause;
  public Tutorials tutorials;
  public GameObject levelsPivot;
  public GameObject finalLevelPivot;
  public MainCamera mainCamera;
  public ThreeBody threeBody;

  Dictionary<object, GameObject> viewModelsDictionary = new Dictionary<object, GameObject>();
  private List<ProgressionView> progressionViews = new List<ProgressionView>();
  private GameObject targetedPlayer = null;

  public GameObject SetupViewObject<T>(
    GameObject prefab,
    Transform parentTransform,
    T model
  ) {
    GameObject gameObject = Instantiate(prefab);
    gameObject.transform.SetParent(parentTransform, true);
    ViewModel<T> viewModel = gameObject.GetComponent<ViewModel<T>>();
    gameObject.SetActive(true);
    viewModel.Setup(model, this);
    viewModelsDictionary.Add(model, gameObject);
    return gameObject;
  }

  public void SetupProgression(Level level) {
    GameObject levelView = GetViewByModel(level);
    foreach (string levelName in ControllerHelpers.PreviousLevels(level.name)) {
      GameObject previousLevelView = GetViewByModel(
        model.levels.FirstOrDefault(l => l.name == levelName)
      );
      GameObject levelProgression = Instantiate(progressionLinePrefab);
      levelProgression.transform.SetParent(levelsPivot.transform, true);
      ProgressionView progressionView = levelProgression.GetComponent<ProgressionView>();
      levelProgression.SetActive(true);
      levelProgression.GetComponent<ProgressionView>().Setup(previousLevelView, levelView);
      progressionViews.Add(levelProgression.GetComponent<ProgressionView>());
    }
  }

  void Awake() {
    Random.InitState(3);
    model.updateEvent.AddListener(HandleModelEvent);
    model.levelCompletedEvent.AddListener(HandleLevelCompletedEvent);
  }
  
  public void Clicked() {
    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

    RaycastHit hit = new RaycastHit();

    if (controller.GetControlMode() == ControlMode.LevelSelect) {
      if (Physics.Raycast (ray, out hit, float.PositiveInfinity, LayerMask.GetMask("Level"))) {
        hit.collider.gameObject.transform.parent.gameObject.GetComponent<Clickable>().OnClick();
      }
    } else {
      if (Physics.Raycast (ray, out hit, float.PositiveInfinity, LayerMask.GetMask("Plane"))) {
        hit.collider.gameObject.transform.parent.gameObject.GetComponent<Plane2DView>().OnClick(hit.point);
      }
    }

    Rerender();
  }

  public void MouseDown() {
    if (controller.GetControlMode() != ControlMode.Edit) return;

    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit = new RaycastHit();

    if (Physics.Raycast (ray, out hit, float.PositiveInfinity, LayerMask.GetMask("Plane"))) {
        hit.collider.gameObject.transform.parent.gameObject.GetComponent<Plane2DView>().OnMouseDown(hit.point);
    }

    Rerender();
  }

  void Update() {
    if (controller.GetControlMode() == ControlMode.LevelSelect) return;
    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

    RaycastHit hit = new RaycastHit();

    if (Physics.Raycast (ray, out hit, float.PositiveInfinity, LayerMask.GetMask("Plane"))) {
      hit.collider.gameObject.transform.parent.gameObject.GetComponent<Plane2DView>().OnHover(hit.point);
    } else {
      mainInput.Unhover();
    }

    Rerender();
  }

  public void Rerender() {
    foreach (GameObject viewModel in viewModelsDictionary.Values) {
      viewModel.GetComponent<Renderable>().Rerender();
    }
    foreach (ProgressionView progressionView in progressionViews) {
      progressionView.Rerender();
    }
  }

  public bool Animating() {
    return (
      viewModelsDictionary.Values.Any(viewModel => viewModel.GetComponent<Renderable>().Animating()) ||
      levelCompletedPause.Animating()
    );
  }

  public GameObject GetViewByModel<T>(T model) where T : class {
    return viewModelsDictionary[model];
  }

  void HandleLevelCompletedEvent(Level level) {
    Rerender();
    levelCompleteSound.GetComponent<AudioSource>().PlayOneShot(levelCompleteSound.GetComponent<AudioSource>().clip, 1f);
    levelCompletedPause.Rerender();
  }

  void HandleModelEvent(
    ModelEventPayload added,
    ModelEventPayload changed,
    ModelEventPayload removed
  ) {
    HandleModelRemoved(removed);
    HandleModelAdded(added);
    Rerender();
  }

  void HandleModelAdded(ModelEventPayload added) {
    foreach (Level level in added.levels) {
      if (level.name == "Final Level") {
        SetupViewObject(finalLevelPrefab, finalLevelPivot.transform, level);
      } else {
        SetupViewObject(levelPrefab, levelsPivot.transform, level);
      }
    }

    foreach (Level level in added.levels) {
      if (level.name != "Final Level") {
        SetupProgression(level);
      }
    }

    foreach (Box3D box3d in added.box3ds) {
      GameObject o = SetupViewObject(box3dPrefab, GetViewByModel(box3d.level).transform, box3d);
      if (box3d.level.name == "Final Level") {
        threeBody.bodies.Add(o);
      }
    }

    if (Globals.mode == Mode.main) {
      threeBody.Setup();
    }

    foreach (Plane2D plane2d in added.planes) {
      GameObject plane2dObject = SetupViewObject(plane2dPrefab, GetViewByModel(plane2d.box).transform, plane2d);
    }
  
    foreach (Object2D player2d in added.player2ds) {
      SetupViewObject(player2dPrefab, GetViewByModel(player2d.plane).transform, player2d);
    }

    foreach (Object2D box2d in added.box2ds) {
      SetupViewObject(box2dPrefab, GetViewByModel(box2d.plane).transform, box2d);
    }

    foreach (Object2D goal2d in added.targets) {
      SetupViewObject(goal2dPrefab, GetViewByModel(goal2d.plane).transform, goal2d);
    }

    foreach (Object2D wall2d in added.walls) {
      SetupViewObject(wall2dPrefab, GetViewByModel(wall2d.plane).transform, wall2d);
    }

    if (added.levels.Count() > 0) {
      tutorials.Setup();
      mainCamera.Setup();
    }
  }

  void HandleModelRemoved(ModelEventPayload removed) {
    foreach (Level obj in removed.levels) {
      GameObject viewModel = GetViewByModel(obj);
      viewModelsDictionary.Remove(obj);
      Destroy(viewModel);
    }
    foreach (Box3D obj in removed.box3ds) {
      GameObject viewModel = GetViewByModel(obj);
      viewModelsDictionary.Remove(obj);
      Destroy(viewModel);
    }
    foreach (Plane2D obj in removed.planes) {
      GameObject viewModel = GetViewByModel(obj);
      viewModelsDictionary.Remove(obj);
      Destroy(viewModel);
    }
    foreach (Object2D obj in removed.player2ds) {
      GameObject viewModel = GetViewByModel(obj);
      viewModelsDictionary.Remove(obj);
      Destroy(viewModel);
    }
    foreach (Object2D obj in removed.box2ds) {
      GameObject viewModel = GetViewByModel(obj);
      viewModelsDictionary.Remove(obj);
      Destroy(viewModel);
    }
    foreach (Object2D obj in removed.targets) {
      GameObject viewModel = GetViewByModel(obj);
      viewModelsDictionary.Remove(obj);
      Destroy(viewModel);
    }
    foreach (Object2D obj in removed.walls) {
      GameObject viewModel = GetViewByModel(obj);
      viewModelsDictionary.Remove(obj);
      Destroy(viewModel);
    }
  }
}
