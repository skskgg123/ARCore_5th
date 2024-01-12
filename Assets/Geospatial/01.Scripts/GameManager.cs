using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AR ī�޶� ���� ��, �� GPS ����(GPSManager)�� ȸ������(CompassManager)�� �������� Pokemon�� �����Ѵ�.
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] List<GameObject> enemyPrefabs;
    [SerializeField] List<Enemy> enemies = new List<Enemy>();
    GPSManager gpsManager;
    CompassManager compassManager;

    private void Awake()
    {
        // AR ī�޶� ���� ��, �� GPS ����(GPSManager)�� ȸ������(CompassManager)�� �������� Pokemon�� �����Ѵ�.
        gpsManager = FindAnyObjectByType<GPSManager>();
        compassManager = FindAnyObjectByType<CompassManager>();
    }

    // CompassManager�� GPSManager�� ������ ��ٸ���.
    // ������ �޾�����, Pokemon�� �����Ѵ�.

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

            PokemonGPS gps = pokemon.GetComponent<Enemy>().PokemonGPS; // ���ϸ��� GPS����
            Vector2 pokemonLocation = new Vector2(gps.latitude, gps.longtitude);
            Vector2 newPokemonLocation = gpsManager.RotationTransformation(pokemonLocation, compassManager.Angle);

            pokemon.transform.position = new Vector3(newPokemonLocation.x, 0, newPokemonLocation.y);
        }
    }
}
