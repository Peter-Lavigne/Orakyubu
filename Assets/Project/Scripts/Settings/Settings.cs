using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.Audio;

public class Settings : MonoBehaviour {
  [System.NonSerialized]
  public SettingsUpdatedEvent settingsUpdatedEvent = new SettingsUpdatedEvent();
  public float musicVolume;
  public float soundVolume;
  public float mouseSensitivity;
  public bool fullScreen;
  public bool invertX;
  public bool invertY;
  public string language = "english";

  private string saveFilePath;
  private Dictionary<string, Localizations> localizations = new Dictionary<string, Localizations>();
  private string[] languages = new string[] {
    "english",
    "schinese",
    "spanish",
    "french",
    "japanese",
    "brazilian"
  };

  void Awake() {
    saveFilePath = Application.persistentDataPath + "/Settings.json";
    ResetToDefaults();
    Load();
    Save();

    try {
      foreach (string l in languages) {
        string filePath = "Localization/" + l;
        localizations[l] = JsonUtility.FromJson<Localizations>(
          Resources.Load<TextAsset>(filePath).text
        );
      }
    } catch (FileNotFoundException e) {}
  }

  void Start() {
    gameObject.SetActive(false);
    InvokeEvent();
  }

  public void InvokeEvent() {
    settingsUpdatedEvent.Invoke();
  }

  void Load() {
    try {
      JsonUtility.FromJsonOverwrite(
        System.IO.File.ReadAllText(saveFilePath),
        this
      );
    } catch (FileNotFoundException e) {}
    UpdateFullScreen();
  }

  public void Save() {
    System.IO.File.WriteAllText(
      saveFilePath,
      JsonUtility.ToJson(this, true)
    );
    settingsUpdatedEvent.Invoke();
  }

  public void Cancel() {
    Load();
    settingsUpdatedEvent.Invoke();
  }

  public void ResetToDefaults() {
    musicVolume = 0.5f;
    soundVolume = 0.5f;
    mouseSensitivity = 0.5f;
    fullScreen = Screen.fullScreen;
    invertX = false;
    invertY = false;
  }

  public void SetFullScreen(bool _fullScreen) {
    fullScreen = _fullScreen;
    UpdateFullScreen();
  }

  public void UpdateFullScreen() {
    if (Application.platform != RuntimePlatform.WebGLPlayer) {
      Screen.fullScreen = fullScreen;
    }
  }

  public string GetLocalization(string key) {
    if (!localizations.ContainsKey(language)) return "";
    return localizations[language].GetLocalizationByName(key);
  }

  public void SetLangauge(string _language) {
    language  = _language;
    Save();
  }
}
