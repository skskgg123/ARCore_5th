using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARCore;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// ARFaceManager의 ARCoreFaceSubsystem 에 접근하여 얼굴 특징점 콘솔에 표시
/// </summary>
[RequireComponent(typeof(ARFaceManager))]
public class FaceDetection : MonoBehaviour
{
    [SerializeField] GameObject faceFeaturePrefab;
    [SerializeField] ARFaceManager faceManager;
    ARCoreFaceSubsystem faceSubsystem;
    List<GameObject> featurePoints = new List<GameObject>(); // 468개의 특징점 Gizmo
    int featureNumber = 468;

    void Start()
    {
        faceManager.facesChanged += OnFaceDetectedEvent;

        faceSubsystem = (ARCoreFaceSubsystem)faceManager.subsystem;

        for(int i = 0; i < featureNumber; i++)
        {
            GameObject featureGO = Instantiate(faceFeaturePrefab);
            featureGO.GetComponentInChildren<TMP_Text>().text = i.ToString();
            featurePoints.Add(featureGO);
        }
    }

    void OnFaceDetectedEvent(ARFacesChangedEventArgs arg)
    {
        if(arg.updated.Count > 0)
        {
            for(int i = 0; i < arg.updated[0].vertices.Length; i++)
            {
                Vector3 vertPos = arg.updated[0].vertices[i];
                Vector3 worldPos = arg.updated[0].transform.TransformPoint(vertPos);

                featurePoints[i].transform.position = worldPos;
                featurePoints[i].gameObject.SetActive(true);
            }
        }
        else if(arg.removed.Count > 0)
        {
            for (int i = 0; i < arg.updated[0].vertices.Length; i++)
            {
                featurePoints[i].gameObject.SetActive(false);
            }
        }
    }
}
