using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] PokemonGPS gps;

    public PokemonGPS PokemonGPS { get { return gps; } }

}
