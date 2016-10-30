using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class CloudProperties
{
    public GameObject cloudPrefab;

    [Header("Distribution")]
    public int numberOfClouds = 200;
    [Space(10)]
    public float minAltitude = 20f;
    public float maxAltitude = 30f;
    [Space(10)]
    public float minSeparation = 40f;

    [Header("Appearance")]
    public float minScale = 0.5f;
    public float maxScale = 2f;
}
