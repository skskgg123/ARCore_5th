using System;

[Serializable]
public class PokemonGPS
{
    public string name;
    public float latitude;
    public float longtitude;
    public bool isCaptured;

    public PokemonGPS(string name, float latitude, float longitude, bool isCaptured)
    {
        this.name = name;
        this.latitude = latitude;
        this.longtitude = longitude;
        this.isCaptured = isCaptured;
    }
}
