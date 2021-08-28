using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuCamera : MonoBehaviour {
  public GameObject mainMenu;
  public Vector3 desiredLocalPositionMain;
  public Vector3 desiredLocalPositionExtras;
  public Vector3 desiredLocalPositionCredits;
  public float moveSpeed;
  public Quaternion desiredLocalRotationMain;
  public Quaternion desiredLocalRotationExtras;
  public Quaternion desiredLocalRotationCredits;
  public float rotationSpeed;

  void Update() {
    MenuState menuState = mainMenu.GetComponent<MainMenu>().menuState;
    Vector3 desiredLocalPosition = menuState == MenuState.Main ? desiredLocalPositionMain : (menuState == MenuState.Extras ? desiredLocalPositionExtras : desiredLocalPositionCredits);
    transform.localPosition = Vector3.MoveTowards(
      transform.localPosition,
      desiredLocalPosition,
      moveSpeed * Time.deltaTime * Vector3.Distance(
        transform.localPosition,
        desiredLocalPosition
      )
    );

    Quaternion desiredLocalRotation = menuState == MenuState.Main ? desiredLocalRotationMain : (menuState == MenuState.Extras ? desiredLocalRotationExtras : desiredLocalRotationCredits);
    
    transform.localRotation = Quaternion.RotateTowards(
      transform.localRotation,
      desiredLocalRotation,
      rotationSpeed * Time.deltaTime * Quaternion.Angle(
        transform.localRotation,
        desiredLocalRotation
      )
    );
  }
}
