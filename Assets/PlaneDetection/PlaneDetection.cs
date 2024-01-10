using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// ARRay를 발사하여 Plane에 닿으면 Indicator를 위치시킨다.
/// </summary>
[RequireComponent(typeof(ARRaycastManager))]
public class PlaneDetection : MonoBehaviour
{
    [SerializeField] Transform indicator;
    [SerializeField] TMP_Text logTxt;
    ARRaycastManager raycastManager;
    Vector2 delta;
    Vector2 screenPoint;

    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        indicator.gameObject.SetActive(false);
    }

    void Update()
    {
        TouchScreen();
    }

    void TouchScreen()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // 스크린의 터치 위치를 확인
                screenPoint = touch.position;
                logTxt.text = touch.position.ToString();
            }
            else if(touch.phase == TouchPhase.Moved)
            {
                delta += touch.deltaPosition;
            }
            else if(touch.phase == TouchPhase.Ended)
            {
                // 터치가 끝났을 때
            }

            LocateIndicatorByScreenTouch();
        }
    }

    void LocateIndicatorByScreenTouch()
    {
        List<ARRaycastHit> hitResults = new List<ARRaycastHit>();

        if(raycastManager.Raycast(screenPoint, hitResults, UnityEngine.XR.ARSubsystems.TrackableType.Planes))
        {
            indicator.transform.position = hitResults[0].pose.position;
            indicator.transform.rotation = hitResults[0].pose.rotation;
            //indicator.forward = -hitResults[0].pose.up;
            indicator.gameObject.SetActive(true);
        }
        else
        {
            indicator.gameObject.SetActive(false);
        }
    }
}
