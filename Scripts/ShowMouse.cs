using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowMouse : MonoBehaviour
{
    private void OnGUI()
    {
        Vector3 mousePosition = Input.mousePosition;

        mousePosition.y = Screen.height - mousePosition.y;

        GUI.Label(new Rect(mousePosition.x, mousePosition.y, 100, 20), "Posición del ratón: " + mousePosition, ToString()); 
    }
}
