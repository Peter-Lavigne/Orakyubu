using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextLocalizer : MonoBehaviour {
  public Settings settings;
  public string key;

  void Awake() {
    if (settings != null) {
      ListenForSettings();
      UpdateText();
    }
  }

  public void ListenForSettings() {
    settings.settingsUpdatedEvent.AddListener(HandleSettingsUpdatedEvent);
  }

  void HandleSettingsUpdatedEvent() {
    UpdateText();
  }

  void UpdateText() {
    GetComponent<Text>().text = settings.GetLocalization(key);
  }
}
