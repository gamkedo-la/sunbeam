using UnityEngine;
using UnityEditor;
using System.Collections;


public class UpdateNestedPrefabsWizard : ScriptableWizard 
{
	[SerializeField] GameObject m_newPrefab;
	[SerializeField] GameObject[] m_objectsToChange;
	

	void OnWizardCreate() 
	{
		int length = m_objectsToChange.Length;
		for (int i = 0; i < m_objectsToChange.Length; i++) 
		{
			var objectToChange = m_objectsToChange[i];
			EditorUtility.DisplayProgressBar("Changing " + objectToChange.name, "checking " + name, (float) i / (float) length);
			ChangeObjectPrefabs(objectToChange, m_newPrefab);
		}

		EditorUtility.ClearProgressBar();
	}

	
	[MenuItem ("Custom/Replace selected objects to prefabs %#u")]
	static void Handler () 
	{
		ScriptableWizard.DisplayWizard<UpdateNestedPrefabsWizard>("Replace selected objects to prefabs", "Apply");
	}

	
	static void ChangeObjectPrefabs (GameObject objectToChange, GameObject newPrefab) 
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


	static void Replace (GameObject originalObject, GameObject newObject) {
		newObject = (GameObject) PrefabUtility.InstantiatePrefab(newObject);
		newObject.transform.position = originalObject.transform.position;
		newObject.transform.rotation = originalObject.transform.rotation;
		newObject.transform.localScale = originalObject.transform.localScale;
		newObject.transform.parent = originalObject.transform.parent;
		DestroyImmediate(originalObject);
	}
}
