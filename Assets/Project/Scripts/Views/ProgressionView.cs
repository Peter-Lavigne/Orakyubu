using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ProgressionView : MonoBehaviour, Renderable {
  public GameObject fromLevel;
  public GameObject toLevel;

  public Gradient neutralColor;
  public Gradient completedColor;

  private static int points = 100;

  public void Setup(GameObject _fromLevel, GameObject _toLevel) {
    fromLevel = _fromLevel;
    toLevel = _toLevel;
    GetComponent<LineRenderer>().positionCount = points;
    SetPositions();
  }

  void SetPositions() {
    List<Vector3> positions = new List<Vector3>();
    for (int i = 0; i < points; i++) {
      positions.Add(
        Vector3.Lerp(
          fromLevel.transform.position + Vector3Int.up * 2 + (fromLevel.transform.rotation * Vector3.right) * 3.5f,
          toLevel.transform.position + Vector3Int.up * 2 + (toLevel.transform.rotation * Vector3.left) * 3.5f,
          (float) i / (float) points
        )
      );
    }
    GetComponent<LineRenderer>().SetPositions(positions.ToArray());
  }
  
  public bool Animating() {
    return false;
  }

  public void Rerender() {
    Level fromLevelModel = fromLevel.GetComponent<LevelView>().model;
    Level toLevelModel = toLevel.GetComponent<LevelView>().model;

    if (!fromLevelModel.IsComplete()) {
      gameObject.SetActive(false);
      return;
    }

    gameObject.SetActive(true);
    GetComponent<LineRenderer>().colorGradient = toLevelModel.IsComplete() ?
      completedColor :
      neutralColor;
  }

  void Update() {
    SetPositions();
  }
}
