using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    void Start()
    {
        if (Time.timeScale != 1f)
        {
            Time.timeScale = 1f;
        }    
    }

}
