using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColorOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject colorable;

    public void OnPointerEnter(PointerEventData eventData) {
        colorable.GetComponent<Text>().color = new Color(0, 206, 255);
    }

    public void OnPointerExit(PointerEventData eventData) {
        colorable.GetComponent<Text>().color = Color.white;
    }
}
