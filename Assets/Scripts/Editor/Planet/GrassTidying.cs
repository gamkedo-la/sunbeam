﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GrassTidying : MonoBehaviour
{
    private static float SeaLevel = 148.9f;
    private static float MaxSlope = 20f;
    private static List<GameObject> ObjectsToDestory;


    private static void DestroyGameObjects()
    {
        int count = ObjectsToDestory.Count;

        for (int i = 0; i < count; i++)
        {
            DestroyImmediate(ObjectsToDestory[i]);
        }

        ObjectsToDestory.Clear();
    }


    [MenuItem("Planet/Delete empty game objects", false, 101)]
    static void DeleteEmptyGameObjects()
    {
        ObjectsToDestory = new List<GameObject>();

        Transform[] transforms = Selection.transforms;

        foreach (Transform myTransform in transforms)
        {
            DeleteEmptyGameObject(myTransform);
        }

        DestroyGameObjects();
    }


    private static void DeleteEmptyGameObject(Transform myTransform)
    {
        if (myTransform.childCount == 0
            && myTransform.GetComponent<MeshRenderer>() == null
            && myTransform.GetComponent<PlanetAlignFlag>() == null)
        {
            ObjectsToDestory.Add(myTransform.gameObject);
        }
        else
        {
            for (int i = 0; i < myTransform.childCount; i++)
            {
                DeleteEmptyGameObject(myTransform.GetChild(i));
            }
        }
    }


    [MenuItem("Planet/Delete game objects below sea level", false, 102)]
    static void DeleteGameObjectsBelowSeaLevel()
    {
        ObjectsToDestory = new List<GameObject>();

        Transform[] transforms = Selection.transforms;

        foreach (Transform myTransform in transforms)
        {
            DeleteGameObjectBelowSeaLevel(myTransform);
        }

        DestroyGameObjects();
        DeleteEmptyGameObjects();
    }


    private static void DeleteGameObjectBelowSeaLevel(Transform myTransform)
    {
        if (myTransform.childCount == 0
            || myTransform.GetChild(0).GetComponent<MeshRenderer>() != null
            || myTransform.GetComponent<PlanetAlignFlag>() != null)
        {
            float distanceFromCentre = myTransform.position.magnitude;

            if (distanceFromCentre < SeaLevel)
                ObjectsToDestory.Add(myTransform.gameObject);
        }

        for (int i = 0; i < myTransform.childCount; i++)
        {
            DeleteGameObjectBelowSeaLevel(myTransform.GetChild(i));
        }
    }


    [MenuItem("Planet/Delete game objects on slope", false, 103)]
    static void DeleteGameObjectsOnSlope()
    {
        ObjectsToDestory = new List<GameObject>();

        Transform[] transforms = Selection.transforms;

        int layerMask = LayerMask.GetMask(Layers.Ground);

        foreach (Transform myTransform in transforms)
        {
            DeleteGameObjectOnSlope(myTransform, layerMask);
        }

        DestroyGameObjects();
        //DeleteEmptyGameObjects();
    }


    private static void DeleteGameObjectOnSlope(Transform myTransform, int layerMask)
    {
        if (myTransform.childCount == 0
            || myTransform.GetChild(0).GetComponent<MeshRenderer>() != null
            || myTransform.GetComponent<PlanetAlignFlag>() != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(myTransform.position + myTransform.up, -myTransform.up, out hit, 10f, layerMask))
            {
                var groundNormal = hit.normal;

                float angle = Vector3.Angle(groundNormal, myTransform.up);

                if (angle > MaxSlope)
                    ObjectsToDestory.Add(myTransform.gameObject);
            }
        }
        else
        {
            for (int i = 0; i < myTransform.childCount; i++)
            {
                DeleteGameObjectOnSlope(myTransform.GetChild(i), layerMask);
            }
        }
    }
}