using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FPSChecker : MonoBehaviour
{
    [SerializeField] TMP_Text fpsTxt;
    float currentTime = 0;
    UniversalRendererData[] scriptableRendererDatas;

    // Update is called once per frame
    void Update()
    {
        currentTime += (Time.unscaledDeltaTime - currentTime) * 0.1f;
        //float mSec = currentTime * 1000;
        float fps = 1.0f / currentTime;
        fpsTxt.text = fps.ToString();
    }
}
