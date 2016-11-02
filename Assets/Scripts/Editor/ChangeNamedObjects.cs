using UnityEngine;
using UnityEditor;
using System.Collections;

public class ChangeNamedObjects : ScriptableWizard
{
    [SerializeField] GameObject m_newPrefab;
    [SerializeField] string m_tagOfObjectsToChange;
    [SerializeField] string m_nameOfObjectsToChange;


    void OnWizardCreate()
    {
        var objectsToChange = GameObject.FindGameObjectsWithTag(m_tagOfObjectsToChange);

        int length = objectsToChange.Length;
        for (int i = 0; i < length; i++)
        {
            var objectToChange = objectsToChange[i];

            if (objectToChange.name != m_nameOfObjectsToChange)
                continue;

            EditorUtility.DisplayProgressBar("Changing " + objectToChange.name, "checking " + name, (float)i / (float)length);
            ChangeObjectPrefabs(objectToChange, m_newPrefab);
        }

        EditorUtility.ClearProgressBar();
    }


    [MenuItem("Custom/Replace named objects with prefabs %#p")]
    static void Handler()
    {
        ScriptableWizard.DisplayWizard<ChangeNamedObjects>("Replace named objects with prefabs", "Apply");
    }


    static void ChangeObjectPrefabs(GameObject objectToChange, GameObject newPrefab)
    {
        var parentTransform = objectToChange.transform.parent;
        var parent = parentTransform != null
            ? objectToChange.transform.parent.gameObject
            : null;

        Replace(objectToChange, newPrefab);

        if (parent != null && PrefabUtility.GetPrefabType(parent) == PrefabType.PrefabInstance)
        {
            PrefabUtility.ReplacePrefab(parent, PrefabUtility.GetPrefabParent(parent));
            PrefabUtility.ReconnectToLastPrefab(parent);
        }
    }


    static void Replace(GameObject originalObject, GameObject newObject)
    {
        newObject = (GameObject)PrefabUtility.InstantiatePrefab(newObject);
        newObject.transform.position = originalObject.transform.position;
        newObject.transform.rotation = originalObject.transform.rotation;
        newObject.transform.localScale = originalObject.transform.localScale;
        newObject.transform.parent = originalObject.transform.parent;
        DestroyImmediate(originalObject);
    }
}
