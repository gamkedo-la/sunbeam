using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FpsDisplay : MonoBehaviour
{
    [SerializeField] float m_calculationTime = 0.5f;

    private Text m_text;
	

    void Awake()
    {
        m_text = GetComponent<Text>();
    }


    void Start()
    {
        if (GameController.AllowCheatMode)
            StartCoroutine(ShowFps());
        else
            m_text.text = "";
    }


    IEnumerator ShowFps()
    {
        int frames = 0;
        float time = 0;

        while (true)
        {
            time += Time.unscaledDeltaTime;
            frames++;

            if (time > m_calculationTime)
            {
                float fps = frames / time;

                m_text.text = string.Format("{0:00}", fps);

                frames = 0;
                time = 0;
            }

            yield return null;
        }
    }
}
