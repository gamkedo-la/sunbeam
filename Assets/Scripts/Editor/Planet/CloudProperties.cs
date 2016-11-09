using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class CloudProperties
{
    public GameObject cloudPrefab;

    [Header("Distribution")]
    public int numberOfClouds = 100;
    [Space(10)]
    public float minAltitude = 40f;
    public float maxAltitude = 50f;
    [Space(10)]
    public float minSeparation = 40f;

    [Header("Appearance")]
    public float minScale = 0.7f;
    public float maxScale = 1.3f;
}
