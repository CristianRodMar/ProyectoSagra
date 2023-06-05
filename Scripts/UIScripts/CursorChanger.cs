using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CursorChanger : MonoBehaviour
{
    public Texture2D cursorTexture; // La textura del cursor personalizado
    public Vector2 hotSpot = Vector2.zero; // El punto caliente (hotspot) del cursor

    void Start()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
    }
}

