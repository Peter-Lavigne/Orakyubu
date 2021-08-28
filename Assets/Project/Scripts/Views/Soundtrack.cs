using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soundtrack : MonoBehaviour {
  private static bool playingMusic;

  void Awake() {
    if (!playingMusic) {
      GetComponent<AudioSource>().Play();
      DontDestroyOnLoad(gameObject);
      playingMusic = true;
    }
  }
}
