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
      "C L I C K   O N   T H E\nC I R C L E   T O   S E L E C T   I T",
      TextAnchor.MiddleLeft
    );
    PlaceTutorial(
      "Intro to 2D",
      0,
      0,
      new Vector2Int(-2, -3),
      6,
      "U S E   W-A-S-D   O R   T H E   A R R O W\nK E Y S   T O   M O V E   I T",
      TextAnchor.MiddleRight
    );
    PlaceTutorial(
      "Intro to 3D",
      0,
      0,
      new Vector2Int(-3, 3),
      6,
      "H O L D   T H E   R I G H T   M O U S E\nB U T T O N   ( O R   S H I F T )",
      TextAnchor.MiddleLeft
    );
    PlaceTutorial(
      "Intro to 3D",
      0,
      0,
      new Vector2Int(-2, -3),
      6,
      "T H E N   M O V E   T H E   M O U S E\nT O   P A N   T H E   C A M E R A",
      TextAnchor.MiddleRight
    );
    PlaceTutorial(
      "Intro to 3D",
      0,
      2,
      new Vector2Int(-3, 3),
      4,
      "U S E   T H E   S C R O L L\nW H E E L   T O   Z O O M",
      TextAnchor.MiddleLeft
    );
    PlaceTutorial(
      "Two Boxes",
      0,
      0,
      new Vector2Int(-3, -1),
      7,
      "U S E   T H E   U P P E R   L E F T\nC O N T R O L S   T O   R E S E T   O R   U N D O",
      TextAnchor.MiddleLeft
    );
    PlaceTutorial(
      "Two Boxes",
      0,
      1,
      new Vector2Int(-3, -1),
      7,
      "Y O U   C A N   A L S O   P R E S S   \" R \"   T O\nR E S E T   O R   \" Z \"   T O   U N D O",
      TextAnchor.MiddleLeft
    );
    PlaceTutorial(
      "Two Cubes",
      0,
      0,
      new Vector2Int(-3, 3),
      5,
      "C L I C K   O N   A   C U B E\nT O   S E L E C T   I T",
      TextAnchor.MiddleLeft
    );
    PlaceTutorial(
      "Two Cubes",
      1,
      0,
      new Vector2Int(-2, -3),
      6,
      "U S E   W-A-S-D   O R   T H E   A R R O W\nK E Y S   T O   M O V E   I T",
      TextAnchor.MiddleRight
    );
  }
}
