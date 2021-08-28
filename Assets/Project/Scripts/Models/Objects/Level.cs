using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Level
{
  public string name;
  public List<Box3D> box3ds;

  private bool completed;

  public void Complete() {
    completed = true;
  }

  public void Uncomplete() {
    completed = false;
  }

  public bool IsComplete() {
    return completed;
  }
}
