using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;

public class Tutorials : MonoBehaviour {
  public Model model;
  public View view;
  public GameObject tutorialPlanePrefab;
  public Settings settings;

  private GameObject GetPlaneView(string levelName, int cubeIndex, int faceIndex) {
    return view.GetViewByModel(
      model.levels.Find(
        level => level.name == levelName
      ).box3ds[cubeIndex].faces[faceIndex]
    );
  }

  private void PlaceTutorial(
    string levelName,
    int cubeIndex,
    int faceIndex,
    Vector2Int startingPosition,
    int widthSpaces,
    string text,
    TextAnchor alignment
  ) {
    GameObject plane = Instantiate(tutorialPlanePrefab);
    Transform plane2d = GetPlaneView(levelName, cubeIndex, faceIndex).transform;
    plane.transform.SetParent(
      plane2d,
      false
    );
    float width = widthSpaces / 7f - .026f;
    float height = 0.117f;
    plane.transform.localPosition = new Vector3(
      1.428f * (startingPosition.x + (widthSpaces - 1f) / 2f),
      1.428f * startingPosition.y,
      -0.001f
    );
    plane.transform.localScale = new Vector3(
      width,
      1f,
      height
    );
    Transform textObject = plane.transform.GetChild(0).GetChild(0);
    textObject.localScale = new Vector3(
      1f / width,
      1f / height,
      1f
    );
    textObject.GetComponent<Text>().text = text;
    textObject.GetComponent<Text>().alignment = alignment;
    textObject.GetComponent<RectTransform>().sizeDelta = new Vector2(widthSpaces * 100f - 30f, 70f);

    if (faceIndex == 2) {
      plane.transform.RotateAround(
        plane2d.position,
        plane2d.rotation * Vector3.back,
        -90f
      );
    } else if (faceIndex == 1) {
      plane.transform.RotateAround(
        plane2d.position,
        plane2d.rotation * Vector3.back,
        90f
      );
    }
  }

  public void Setup() {
    if (Globals.mode != Mode.main) return;
    PlaceTutorial(
      "Intro to 2D",
      0,
      0,
      new Vector2Int(-3, 3),
      5,
      settings.GetLocalization("tutorial_move_circle_1"),
      TextAnchor.MiddleLeft
    );
    PlaceTutorial(
      "Intro to 2D",
      0,
      0,
      new Vector2Int(-2, -3),
      6,
      settings.GetLocalization("tutorial_move_circle_2"),
      TextAnchor.MiddleRight
    );
    PlaceTutorial(
      "Intro to 3D",
      0,
      0,
      new Vector2Int(-3, 3),
      6,
      settings.GetLocalization("tutorial_camera_1"),
      TextAnchor.MiddleLeft
    );
    PlaceTutorial(
      "Intro to 3D",
      0,
      0,
      new Vector2Int(-2, -3),
      6,
      settings.GetLocalization("tutorial_camera_2"),
      TextAnchor.MiddleRight
    );
    PlaceTutorial(
      "Intro to 3D",
      0,
      2,
      new Vector2Int(-3, 3),
      4,
      settings.GetLocalization("tutorial_zoom"),
      TextAnchor.MiddleLeft
    );
    PlaceTutorial(
      "Two Boxes",
      0,
      0,
      new Vector2Int(-3, -1),
      7,
      settings.GetLocalization("tutorial_reset_undo_1"),
      TextAnchor.MiddleLeft
    );
    PlaceTutorial(
      "Two Boxes",
      0,
      1,
      new Vector2Int(-3, -1),
      7,
      settings.GetLocalization("tutorial_reset_undo_2"),
      TextAnchor.MiddleLeft
    );
    PlaceTutorial(
      "Two Cubes",
      0,
      0,
      new Vector2Int(-3, 3),
      5,
      settings.GetLocalization("tutorial_move_cube_1"),
      TextAnchor.MiddleLeft
    );
    PlaceTutorial(
      "Two Cubes",
      1,
      0,
      new Vector2Int(-2, -3),
      6,
      settings.GetLocalization("tutorial_move_cube_2"),
      TextAnchor.MiddleRight
    );
  }
}
