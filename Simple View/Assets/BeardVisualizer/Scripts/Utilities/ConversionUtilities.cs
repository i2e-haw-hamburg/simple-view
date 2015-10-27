using UnityEngine;
using System.Collections;

public static class ConversionUtilities {

    public static Vector3 NetworkWorldCoordinatesToUnityCoordinates(Vector3 networkCoordinates)
    {
        return networkCoordinates;
        //return new Vector3(networkCoordinates.x, networkCoordinates.z, -networkCoordinates.y);
    }
}
