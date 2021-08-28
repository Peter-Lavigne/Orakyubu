using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ThreeBody : MonoBehaviour {
  public Model model;
  public float period;
  public float xAmplitude;
  public float yAmplitude;
  public float zAmplitude;

  private float scale = 25f;
  private float offset = 62.5f;

  public List<GameObject> bodies;

  void UpdatePositions() {
    if (!model.finalLevelAvailable) {
      for (int i = 0; i < bodies.Count(); i++) {
        float t = Time.time / period + (2 * Mathf.PI / bodies.Count() * i);
        bodies[i].transform.localPosition = new Vector3(
          xAmplitude * Mathf.Cos(t) / scale,
          yAmplitude * Mathf.Cos(t) / scale + (offset / scale),
          zAmplitude * Mathf.Sin(2*t) / 2 / scale
        );
      }
    }
  }

  public void Setup() {
    UpdatePositions();
  }

  void Update() {
    UpdatePositions();
  }
}
