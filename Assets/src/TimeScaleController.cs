﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleController : MonoBehaviour
{

    [Range(0, 5)]
    public int timeScale = 1;


    // Update is called once per frame
    void Update()
    {
        Time.timeScale = timeScale;
    }
}
