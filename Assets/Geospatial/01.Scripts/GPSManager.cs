using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Android;

// ����̽��� GPS������ ��ũ���� UI Text�� ����ش�.
// 1. Permission ����
// 2. GPS ������ Location ���� �ʱ�ȭ
// 3. GPS ������ UI Text�� ����ֱ�
// * 10���� �̻� �־����� ��� GPS ���� ������Ʈ
public class GPSManager : MonoBehaviour
{
    [SerializeField] TMP_Text logTxt;
    [SerializeField] TMP_Text latitudeTxt;
    [SerializeField] TMP_Text longtitudeTxt;
    float latitude;
    float longtitude;
    Vector2 myLocation;
    public Vector2 MyLocation { get { return myLocation; } }
    CompassManager compassManager;
    bool isDataReciver;
    public bool IsDataReciver { get { return isDataReciver; } }


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
        // 1. Permission ����
        if(!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            // ��ġȮ�� ���� ��û
            Permission.RequestUserPermission(Permission.FineLocation);

            yield return new WaitForSeconds(3);

            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                logTxt.text = "��ġ ���� ������ ������ּ���. 3�� �Ŀ� ���ø����̼��� ����˴ϴ�.";

                Application.Quit();
            }
        }

        if(!Input.location.isEnabledByUser)
        {
            logTxt.text = "��ġ ���� ������ ������ּ���. 3�� �Ŀ� ���ø����̼��� ����˴ϴ�.";

            yield return new WaitForSeconds(3);

            Application.Quit();
        }
        else
        {
            logTxt.text = "GPS is allowed";
        }

        // 2. GPS ������ Location ���� �ʱ�ȭ
        // ����̽��� GPS ���� ����
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

        isDataReciver = true;

        // 3. GPS ������ UI Text�� ����ֱ�
        // Latitude: 37.51406
        // Longtitude: 127.0295
        while (true)
        {
            latitude = Input.location.lastData.latitude;
            longtitude = Input.location.lastData.longitude;
            myLocation = new Vector2(latitude, longtitude);
            latitudeTxt.text = "Latitude: " + latitude.ToString();
            longtitudeTxt.text = "Longtitude: " + longtitude.ToString();

            logTxt.text = "";

            CheckGPSInfo();

            yield return new WaitForSeconds(1);

            Input.location.Start();
        }
    }

    void CheckGPSInfo()
    {
        foreach (var pokemon in pokemonInfo)
            {
                Vector2 from = new Vector2(latitude, longtitude);
                Vector2 to = new Vector2(pokemon.latitude, pokemon.longtitude);

                float distance = CalculateDistance(from, to);

                logTxt.text += $"{pokemon.name}�� {distance}���� �ְ�, ��ȹ �Ǿ����ϱ�? {pokemon.isCaptured}\n";

                if (distance < 10)
                {
                    // �̰����� Ư�� ����� ����
                    // ��) StartBattle();
                }

            }
    }

    // GPS A���� GPS B ������ �Ÿ��� ���
    /*    37.513800, 127.021600(GPS A)
          37.513600, 127.020500(GPS B)
          0.0002(20m), 0.0011(110m)
          * 100000
          0.0002, 0.009 -> 20m, 110m   */
    /// <summary>
    /// GPS A���� GPS B ������ �Ÿ��� ���
    /// </summary>
    /// <param name="from">GPS A�� ��ǥ</param>
    /// <param name="to">GPS B�� ��ǥ</param>
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
    /// XROrigin�� RealWorld ȸ�� ������ �������� Ÿ�ٿ�����Ʈ�� ��ȯ�� ��ǥ�� ��ȯ
    /// </summary>
    /// <param name="targetCoordinates">Ÿ�ٿ�����Ʈ�� ��ǥ</param>
    /// <param name="angle">CompassManager ������ XROrigin ȸ�� ����</param>
    /// <returns></returns>
   public Vector2 RotationTransformation(Vector2 targetCoordinates, float angle)
    {
        angle += Mathf.Deg2Rad;

        // �� GPS ��ǥ ����, Pokemon ������Ʈ�� GPS(x1, y1)�� ���
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
