using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class UnlockAllLevels : MonoBehaviour {
  public void UnlockAll() {
    CreateCompletedProgress();
    Scene scene = SceneManager.GetActiveScene();
    SceneManager.LoadScene(scene.name);
  }

  void CreateCompletedProgress() {
    Progress progress = new Progress();

    List<Level> levels = JsonUtility.FromJson<SavableModel>(
      Resources.Load<TextAsset>("StartingModel").text
    ).levels;

    foreach (Level level in levels) {
      if (level.name != "Final Level") {
        progress.completedLevels.Add(level.name);
      }
    }

    System.IO.File.WriteAllText(
      Application.persistentDataPath + "/Progress.json",
      JsonUtility.ToJson(progress, true)
    );
  }
}