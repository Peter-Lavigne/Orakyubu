using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Credits : MonoBehaviour {
  public Controller controller;
  public Button menuButton;
  public GameObject creditsObject;
  private float creditsTimer = -7.5f;
  private float secondsBetweenItems = 5f;
  private float fadeInTime = 1.5f;

  public List<CanvasGroup> creditItems;

  void Update() {
    if (controller.GetControlMode() != ControlMode.Credits) return;

    creditsObject.SetActive(true);

    creditsTimer += Time.deltaTime;

    for (int i = 0; i < creditItems.Count; i++) {
      if (i * secondsBetweenItems < creditsTimer) {
        creditItems[i].alpha = Mathf.Clamp(
          (creditsTimer - (i * secondsBetweenItems)) / fadeInTime,
          0f,
          1.0f
        );
      }
    }

    if (menuButton.interactable == false && creditsTimer > (creditItems.Count - 1) * secondsBetweenItems) {
      menuButton.interactable = true;
    }
  }
}
