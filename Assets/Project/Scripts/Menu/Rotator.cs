using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
  public float rotationsPerMinute;

  void Update() {
    transform.Rotate(
      0,
      6.0f * rotationsPerMinute * Time.deltaTime,
      0
    );
  }
}
