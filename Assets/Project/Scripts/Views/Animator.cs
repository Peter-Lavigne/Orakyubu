using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
public class Animator
{
  public float totalSeconds;
  private float timeElapsed;
  private bool animating = false;

  public bool Animating() {
    return animating;
  }

  public void Start() {
    animating = true;
    timeElapsed = 0;
  }

  public void Update(Action<float> updateAction, Action finishAction) {
    if (!Animating()) return;

    if (timeElapsed < totalSeconds) {
      timeElapsed += Time.deltaTime;
      updateAction(timeElapsed / totalSeconds);
    } else {
      animating = false;
      finishAction();
    }
  }
}
