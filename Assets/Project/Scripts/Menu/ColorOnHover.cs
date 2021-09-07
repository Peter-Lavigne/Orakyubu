using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColorOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject colorable1;
    public GameObject colorable2;

    public void OnPointerEnter(PointerEventData eventData) {
        colorable1.GetComponent<Text>().color = new Color(0, 206, 255);
        if (colorable2 != null) {
            colorable2.GetComponent<Text>().color = new Color(0, 206, 255);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        colorable1.GetComponent<Text>().color = Color.white;
        if (colorable2 != null) {
            colorable2.GetComponent<Text>().color = Color.white;
        }
    }
}
