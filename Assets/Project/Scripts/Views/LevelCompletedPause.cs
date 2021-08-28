using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCompletedPause : MonoBehaviour, Renderable {
  public Controller controller;
  public Animator waitAnimator = new Animator();

  public void Rerender() {
    if (Globals.mode != Mode.main) return;
    waitAnimator.Start();
  }

  public bool Animating() {
    return waitAnimator.Animating();
  }

  void Update() {
    if (!Animating()) return;
    waitAnimator.Update(
      t => {},
      () => {
        if (controller.GetControlMode() != ControlMode.Credits) {
          controller.SetControlMode(ControlMode.LevelSelect);
        }
      }
    );
  }
}
