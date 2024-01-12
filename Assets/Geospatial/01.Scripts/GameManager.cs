using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AR 카메라가 실행 후, 내 GPS 정보(GPSManager)와 회전각도(CompassManager)를 기준으로 Pokemon을 생성한다.
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] List<GameObject> enemyPrefabs;
    [SerializeField] List<Enemy> enemies = new List<Enemy>();
    GPSManager gpsManager;
    CompassManager compassManager;

    private void Awake()
    {
        // AR 카메라 실행 후, 내 GPS 정보(GPSManager)와 회전각도(CompassManager)를 기준으로 Pokemon을 생성한다.
        gpsManager = FindAnyObjectByType<GPSManager>();
        compassManager = FindAnyObjectByType<CompassManager>();
    }

    // CompassManager와 GPSManager의 정보를 기다린다.
    // 정보가 받아지면, Pokemon을 스폰한다.

    IEnumerator CheckSensorStatus()
    {
        yield return new WaitUntil(() => gpsManager == true && compassManager == true);

        SpawnEnemy();
    }

    void SpawnEnemy()
    {
        Vector2 myLocation = gpsManager.MyLocation;

        foreach (var prefab in enemyPrefabs)
        {
            GameObject pokemon = Instantiate(prefab);

            PokemonGPS gps = pokemon.GetComponent<Enemy>().PokemonGPS; // 포켓몬의 GPS정보
            Vector2 pokemonLocation = new Vector2(gps.latitude, gps.longtitude);
            Vector2 newPokemonLocation = gpsManager.RotationTransformation(pokemonLocation, compassManager.Angle);

            pokemon.transform.position = new Vector3(newPokemonLocation.x, 0, newPokemonLocation.y);
        }
    }
}
