using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Android;

// 디바이스의 GPS정보를 스크린의 UI Text에 띄워준다.
// 1. Permission 설정
// 2. GPS 센서의 Location 상태 초기화
// 3. GPS 정보를 UI Text로 띄워주기
// * 10미터 이상 멀어졌을 경우 GPS 정보 업데이트
public class GPSManager : MonoBehaviour
{
    [SerializeField] TMP_Text logTxt;
    [SerializeField] TMP_Text latitudeTxt;
    [SerializeField] TMP_Text longtitudeTxt;
    float latitude;
    float longtitude;
    CompassManager compassManager;

    [Serializable]
    class PokemonGPS
    {
        public string name;
        public float latitude;
        public float longitude;
        public bool isCaptured;

        public PokemonGPS(string name, float latitude, float longitude, bool isCaptured)
        {
            this.name = name;
            this.latitude = latitude;
            this.longitude = longitude;
            this.isCaptured = isCaptured;
        }
    }

    [SerializeField] List<PokemonGPS> pokemonInfo = new List<PokemonGPS>();
    private void Awake()
    {
        compassManager = FindAnyObjectByType<CompassManager>();
    }

    void Start()
    {
        StartCoroutine(TurnOnGPS());
    }

    IEnumerator TurnOnGPS()
    {
        // 1. Permission 설정
        if(!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            // 위치확인 권한 요청
            Permission.RequestUserPermission(Permission.FineLocation);

            yield return new WaitForSeconds(3);

            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                logTxt.text = "위치 정보 권한을 허용해주세요. 3초 후에 어플리케이션이 종료됩니다.";

                Application.Quit();
            }
        }

        if(!Input.location.isEnabledByUser)
        {
            logTxt.text = "위치 정보 권한을 허용해주세요. 3초 후에 어플리케이션이 종료됩니다.";

            yield return new WaitForSeconds(3);

            Application.Quit();
        }
        else
        {
            logTxt.text = "GPS is allowed";
        }

        // 2. GPS 센서의 Location 상태 초기화
        // 디바이스의 GPS 서비스 시작
        Input.location.Start();

        while(Input.location.status == LocationServiceStatus.Initializing)
        {
            logTxt.text = "GPS is Initializing";

            yield return new WaitForSeconds(1);
        }

        if(Input.location.status != LocationServiceStatus.Failed) 
        {
            logTxt.text = "GPS Initialization Failed";
        }

        // 3. GPS 정보를 UI Text로 띄워주기
        // Latitude: 37.51406
        // Longtitude: 127.0295
        while (true)
        {
            latitude = Input.location.lastData.latitude;
            longtitude = Input.location.lastData.longitude;
            latitudeTxt.text = "Latitude: " + latitude.ToString();
            longtitudeTxt.text = "Longtitude: " + longtitude.ToString();

            logTxt.text = "";

            foreach (var pokemon in pokemonInfo)
            {
                Vector2 from = new Vector2(latitude, longtitude);
                Vector2 to = new Vector2(pokemon.latitude, pokemon.longitude);

                float distance = CalculateDistance(from, to);

                logTxt.text += $"{pokemon.name}은 {distance}내에 있고, 포획 되었습니까? {pokemon.isCaptured}\n";

                if (distance < 10)
                {
                    // 이곳에서 특정 기능을 실행
                    // 예) StartBattle();
                }

                RotationTransformation(to, compassManager.Angle * Mathf.Deg2Rad);
            }

            yield return new WaitForSeconds(1);

            Input.location.Start();
        }
    }

    // GPS A에서 GPS B 사이의 거리를 계산
    /*    37.513800, 127.021600(GPS A)
          37.513600, 127.020500(GPS B)
          0.0002(20m), 0.0011(110m)
          * 100000
          0.0002, 0.009 -> 20m, 110m   */
    /// <summary>
    /// GPS A에서 GPS B 사이의 거리를 계산
    /// </summary>
    /// <param name="from">GPS A의 좌표</param>
    /// <param name="to">GPS B의 좌표</param>
    /// <returns></returns>
    float CalculateDistance(Vector2 from, Vector2 to)
    {
        float distance = 0;

        float a = (from.y - to.y);
        float b = (from.x - to.x);
        float c = Mathf.Sqrt(Mathf.Pow(a, 2) + Mathf.Pow(b, 2));
        distance = c * 100000;

        return distance;
    }

    /// <summary>
    /// XROrigin의 RealWorld 회전 각도를 기준으로 타겟오브젝트의 변환된 좌표를 반환
    /// </summary>
    /// <param name="targetCoordinates">타겟오브젝트의 좌표</param>
    /// <param name="angle">CompassManager 기준의 XROrigin 회전 각도</param>
    /// <returns></returns>
    Vector2 RotationTransformation(Vector2 targetCoordinates, float angle)
    {
        // 내 GPS 좌표 기준, Pokemon 오브젝트의 GPS(x1, y1)를 계산
        float x1 = (targetCoordinates.x - latitude) * 100000; // 0.0002 * 100000 = 2
        float y1 = (targetCoordinates.y - longtitude) * 100000; // 0.0011 * 100000 = 110

        float x2 = (Mathf.Cos(angle) * x1) + (-Mathf.Sin(angle) * y1); // 
        float y2 = (Mathf.Sin(angle) * x1) + (Mathf.Cos(angle) * y1);

        Vector2 newTargetCoordinates = new Vector2(x2, y2);

        logTxt.text += $"Angle({angle}), Before({x1}, {y1}), After({x2}, {y2})\n"; 

        return newTargetCoordinates;
    }


    private void OnApplicationQuit()
    {
        // Stops the location service if there is no need to query location updates continuously.
        Input.location.Stop();
    }
}
