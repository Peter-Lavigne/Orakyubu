using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ViewModel<T> : MonoBehaviour, Renderable
{
  public T model;
  public View view;
  public MainInput mainInput;

  public virtual void Setup(T _model, View _view)
  {
    model = _model;
    view = _view;
  }

  public abstract void Rerender();

  public abstract bool Animating();
}
