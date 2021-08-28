using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Move
{
  bool Apply();
  void Undo();
  void InvokeEvent();
}
