using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Mode {
  main, editor
};

public class Globals {
  public static Mode mode = Mode.main;
}
