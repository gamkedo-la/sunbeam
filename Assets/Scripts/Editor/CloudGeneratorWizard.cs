using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CloudGeneratorWizard : ScriptableWizard
{
    [SerializeField] int m_seed = 1;
    [SerializeField] CloudProperties[] m_clouds = new CloudProperties[1];
    [SerializeField] float m_planetRadius = 200;

    private int m_maxAttempts = 20;
    private List<Transform> m_placedClouds = new List<Transform>();


    void OnWizardCreate()
    {
        Random.seed = m_seed;
        int totalClouds = 0;

        for (int i = 0; i < m_clouds.Length; i++)
            totalClouds += m_clouds[i].numberOfClouds;

        var parent = new GameObject();
        parent.name = "Clouds";

        int cloudsPlaced = 0;

        for (int i = 0; i < m_clouds.Length; i++)
        {
            var cloudProperties = m_clouds[i];

            var cloudTypeParent = new GameObject();
            cloudTypeParent.name = "Cloud type: " + cloudProperties.cloudPrefab.name;
            cloudTypeParent.transform.parent = parent.transform;
 
            int numClouds = cloudProperties.numberOfClouds;
            for (int j = 0; j < numClouds; j++)
            {
                EditorUtility.DisplayProgressBar("Generating clouds", "Percentage clouds placed: ", (float) cloudsPlaced / totalClouds);

                PlaceCloud(cloudProperties, cloudTypeParent.transform);
                cloudsPlaced++;
            }
        }

        EditorUtility.ClearProgressBar();
    }


    [MenuItem("Custom/Generate clouds %#g")]
    static void Handler()
    {
        DisplayWizard<CloudGeneratorWizard>("Generate clouds", "Gnerate clouds");
    }


    private void PlaceCloud(CloudProperties cloudProperties, Transform parent)
    {
        var cloudPrefab = cloudProperties.cloudPrefab;

        for (int i = 0; i < m_maxAttempts; i++)
        {
            var position = Random.onUnitSphere;
            float radius = Random.Range(m_planetRadius + cloudProperties.minAltitude, m_planetRadius + cloudProperties.maxAltitude);
            position *= radius;

            bool success = TestPosition(position, cloudProperties.minSeparation);

            if (!success)
                continue;

            var cloud = Instantiate(cloudPrefab);
            cloud.transform.position = position;

            float rotation = Random.Range(0f, 360f);
            cloud.transform.Rotate(Vector2.up, rotation);

            float scale = Random.Range(cloudProperties.minScale, cloudProperties.maxScale);
            cloud.transform.localScale = scale * Vector3.one;

            var targetDirection = cloud.transform.position.normalized;
            var cloudUp = cloud.transform.up;
            cloud.transform.rotation = Quaternion.FromToRotation(cloudUp, targetDirection) * cloud.transform.rotation;

            cloud.transform.parent = parent;
            m_placedClouds.Add(cloud.transform);
            break;
        }
    }


    private bool TestPosition(Vector3 position, float minSeparation)
    {
        for (int i = 0; i < m_placedClouds.Count; i++)
        {
            float separation = Vector3.Distance(position, m_placedClouds[i].position);

            if (separation < minSeparation)
                return false;
        }

        return true;
    }
}
