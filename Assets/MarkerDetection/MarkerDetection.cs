using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
public class MarkerDetection : MonoBehaviour
{
    [SerializeField] Transform obj;
    float delta;
    Coroutine rotCoroutine;
    ARTrackedImageManager imageManager;

    private void Awake()
    {
        imageManager = GetComponent<ARTrackedImageManager>();

        imageManager.trackedImagesChanged += OnImageTeackedEvent;
    }

    void OnImageTeackedEvent(ARTrackedImagesChangedEventArgs args)
    {
        foreach(ARTrackedImage trackedImage in args.added)
        {
            obj = trackedImage.transform;
        }
    }

    public void OnButtonDragEvent(int angle)
    {
        delta += angle;
        obj.rotation = Quaternion.Euler(0, delta, 0);
    }

    IEnumerator RotateObj(int angle)
    {
        while(true)
        {
            delta += angle;
            obj.rotation = Quaternion.Euler(0, delta, 0);

            yield return new WaitForSeconds(0.05f);
        }    
    }

    public void OnButtonDownEvent(int angle)
    {
        // 코루틴 함수 실행
        rotCoroutine = StartCoroutine(RotateObj(angle));
    }

    public void OnButtonUpEvent()
    {
        // 코루틴 종료
        StopCoroutine(rotCoroutine);
    }
}

