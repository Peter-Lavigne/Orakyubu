using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ModelEvent : UnityEvent<
  ModelEventPayload, // added
  ModelEventPayload, // changed
  ModelEventPayload  // removed
> {}
