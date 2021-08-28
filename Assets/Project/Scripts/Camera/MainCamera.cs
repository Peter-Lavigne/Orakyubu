using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MainCamera : MonoBehaviour {
  public Controller controller;
  public View view;
  public GameObject pivot;
  public GameObject camera;
  public float pivotRotationSpeed;
  public float pivotSpeed;
  public float cameraSpeed;
  public Settings settings;
  public GameObject levelsPivot;
  public float levelSelectDistance;
  public float marginPercentage = 1.1f;
  public float rotateSensitivtyConstant = 0.8f;
  public Model model;

  private bool finishedSetup = false;
  private Vector3 desiredPivotLocalPosition;
  private Quaternion desiredPivotLocalRotation;
  private Vector3 desiredCameraLocalPosition;

  private float creditsTimer = -2.5f;

  public Face FacingFace() {
    float yRotation = pivot.transform.localRotation.eulerAngles.y;
    if (yRotation < 45 || yRotation > 315) {
      return Face.Front;
    } else if (yRotation < 135) {
      return Face.Left;
    } else if (yRotation < 225) {
      return Face.Back;
    } else {
      return Face.Right;
    }
  }

  void MoveTowardsDesired() {
    if (controller.GetControlMode() == ControlMode.Credits) {
      creditsTimer += Time.deltaTime;
      if (creditsTimer > 0) {
        camera.transform.localPosition = camera.transform.localPosition + (camera.transform.localRotation * Vector3.back) * Time.deltaTime * (creditsTimer < 2.5f ? 5f * creditsTimer / 2.5f : 5f);
      }
      return;
    }

    pivot.transform.localRotation = Quaternion.RotateTowards(
      pivot.transform.localRotation,
      desiredPivotLocalRotation,
      pivotRotationSpeed * Time.deltaTime * Quaternion.Angle(
        pivot.transform.localRotation,
        desiredPivotLocalRotation
      )
    );

    pivot.transform.localPosition = Vector3.MoveTowards(
      pivot.transform.localPosition,
      desiredPivotLocalPosition,
      pivotSpeed * Time.deltaTime * Vector3.Distance(
        pivot.transform.localPosition,
        desiredPivotLocalPosition
      )
    );

    camera.transform.localPosition = Vector3.MoveTowards(
      camera.transform.localPosition,
      desiredCameraLocalPosition,
      Vector3.Distance(camera.transform.localPosition, desiredCameraLocalPosition) * Time.deltaTime * cameraSpeed
    );
  }

  private IEnumerable<Vector3> CubeCorners(Vector3 center, bool finalLevel) {
    List<Vector3> points = new List<Vector3>();
    foreach (bool posX in new [] { true, false }) {
    foreach (bool posY in new [] { true, false }) {
    foreach (bool posZ in new [] { true, false }) {
      float distance = 0.5f * (finalLevel ? 50f : 1f);
      points.Add(new Vector3(
        center.x + (posX ? distance : -distance),
        center.y + (posY ? distance : -distance),
        center.z + (posZ ? distance : -distance)
      ));
    }}}
    return points;
  }

  // adapted from https://forum.unity.com/threads/fit-object-exactly-into-perspective-cameras-field-of-view-focus-the-object.496472/
  float CameraDistance() {
    if (controller.GetControlMode() == ControlMode.LevelSelect) return levelSelectDistance;

    IEnumerable<Vector3> cornerPoints = controller.GetCurrentLevel().box3ds.SelectMany(box3d => CubeCorners(view.GetViewByModel(box3d).transform.localPosition, Globals.mode == Mode.main && controller.GetCurrentLevel().name == "Final Level"));
    float maxExtent = cornerPoints.Select(v => Vector3.Distance(v, desiredPivotLocalPosition)).Max();
    return (maxExtent * marginPercentage) / Mathf.Sin(Mathf.Deg2Rad * camera.GetComponent<Camera>().fieldOfView / 2f);
  }

  void UpdateDesiredCameraPosition() {
    desiredCameraLocalPosition = Vector3.Normalize(new Vector3(0, 0, -2f)) * CameraDistance();
  }

  public void Zoom(float amount) {
    if (controller.GetControlMode() == ControlMode.LevelSelect) return;
    marginPercentage -= amount;
    marginPercentage = Mathf.Clamp(marginPercentage, 0.9f, 1.2f);
  }

  void UpdateDesiredPivotLocalPosition() {
    if (controller.GetControlMode() == ControlMode.LevelSelect) {
      desiredPivotLocalPosition = levelsPivot.transform.localPosition;
      return;
    }

    IEnumerable<Vector3> box3dViewPositions = controller.GetCurrentLevel().box3ds.Select(box3d => view.GetViewByModel(box3d).transform.localPosition);
    desiredPivotLocalPosition = box3dViewPositions.Aggregate(
      new Vector3(0,0,0),
      (total, current) => total + current
    ) / ((float) box3dViewPositions.Count());
  }

  void Update() {
    UpdateDesiredPivotLocalPosition();
    UpdateDesiredCameraPosition();
    MoveTowardsDesired();
    camera.transform.LookAt(pivot.transform, pivot.transform.parent.rotation * Vector3.up);
  }

  private float ClampEulerAngle(float eulerAngle, float max, float min) {
    if (eulerAngle < 0) eulerAngle = 360f + eulerAngle;
    if (
      eulerAngle <= max ||
      eulerAngle >= 360f + min
    ) {
      return eulerAngle;
    }
    return eulerAngle < 180f ? max : min;
  }

  // return max or min x-axis rotation limits
  public float RotationLimit(bool max) {
    if (controller.GetControlMode() == ControlMode.LevelSelect) return max ? ( model.finalLevelAvailable ? 20f : 6f) : -6f;
    if (!max && desiredPivotLocalPosition.y < 2) {
      return 0;
    }
    if (max && desiredPivotLocalPosition.y > 4) {
      return 0;
    }
    return max ? 48f : -48f;
  }

  public void RotatePivot(float xAmount, float yAmount) {
    if (Mathf.Abs(Quaternion.Angle(pivot.transform.localRotation, desiredPivotLocalRotation)) > 5f) return;
    pivot.transform.localRotation = Quaternion.Euler(
      ClampEulerAngle(
        pivot.transform.localRotation.eulerAngles.x + xAmount * rotateSensitivtyConstant,
        RotationLimit(true),
        RotationLimit(false)
      ),
      pivot.transform.localRotation.eulerAngles.y + yAmount * rotateSensitivtyConstant,
      0
    );
    desiredPivotLocalRotation = pivot.transform.localRotation;
  }

  void Awake() {
    controller.controlModeChangedEvent.AddListener(HandleControlModeChangedEvent);

    float[] distances = new float[32];
    distances[20] = 100;
    camera.GetComponent<Camera>().layerCullDistances = distances;

  }

  public void Setup() {
    if (controller.GetControlMode() == ControlMode.Puzzle || controller.GetControlMode() == ControlMode.Edit) {
      pivot.transform.SetParent(view.GetViewByModel(controller.GetCurrentLevel()).transform);
      desiredPivotLocalRotation = Quaternion.identity;
    } else {
      pivot.transform.SetParent(levelsPivot.transform);
      Level lastUncompletedLevel = ControllerHelpers.levelOrder.SelectMany(l => l).Select(levelName => model.levels.FirstOrDefault(level => level.name == levelName)).Where(level => ControllerHelpers.PreviousLevels(level.name).Select(levelName => model.levels.FirstOrDefault(level => level.name == levelName)).Any(level => level.IsComplete())).LastOrDefault(level => !level.IsComplete());
      if (lastUncompletedLevel == null) {
        if (model.finalLevelAvailable) {
          desiredPivotLocalRotation = Quaternion.Euler(20f, 0f, 0f);
        } else {
          desiredPivotLocalRotation = Quaternion.identity;
        }
      } else {
        desiredPivotLocalRotation = view.GetViewByModel(lastUncompletedLevel).transform.localRotation;
      }
    }
    UpdateDesiredPivotLocalPosition();
    UpdateDesiredCameraPosition();
    pivot.transform.localRotation = desiredPivotLocalRotation;
    pivot.transform.localPosition = desiredPivotLocalPosition;
    camera.transform.localPosition = desiredCameraLocalPosition;
    finishedSetup = true;
  }

  void HandleControlModeChangedEvent(ControlMode controlMode) {
    if (!finishedSetup) return;

    if (controlMode == ControlMode.Puzzle && controller.GetCurrentLevel().name == "Final Level") {
      camera.GetComponent<Camera>().nearClipPlane = 30f;
    } else {
      camera.GetComponent<Camera>().nearClipPlane = 0.3f;
    }

    if (controlMode == ControlMode.LevelSelect) {
      pivot.transform.SetParent(levelsPivot.transform);
      if (model.finalLevelAvailable && !model.levels.First(l => l.name == "Final Level").IsComplete()) {
        Quaternion baseRotation = view.GetViewByModel(controller.GetCurrentLevel()).transform.localRotation;
        desiredPivotLocalRotation = Quaternion.Euler(
          20f,
          baseRotation.eulerAngles.y,
          0f
        );
      } else {
        Level unsolvedNextLevel = ControllerHelpers.NextLevels(controller.GetCurrentLevel().name).Select(levelName => model.levels.FirstOrDefault(l => l.name == levelName)).FirstOrDefault(level => !level.IsComplete());
        desiredPivotLocalRotation = view.GetViewByModel((unsolvedNextLevel == null || !controller.GetCurrentLevel().IsComplete()) ? controller.GetCurrentLevel() : unsolvedNextLevel).transform.localRotation;
      }
    } else if (controlMode == ControlMode.Puzzle || controlMode == ControlMode.Edit) {
      pivot.transform.SetParent(view.GetViewByModel(controller.GetCurrentLevel()).transform);
      desiredPivotLocalRotation = controller.GetCurrentLevel().name == "Final Level" ? Quaternion.Euler(45f, 45f, 0f) : Quaternion.identity;
    } else if (controlMode == ControlMode.Credits) {
      pivot.transform.SetParent(levelsPivot.transform.parent);
      desiredPivotLocalRotation = pivot.transform.localRotation;
    }
  }
}
