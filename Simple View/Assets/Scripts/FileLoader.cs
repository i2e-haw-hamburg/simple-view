using STPLoader;
using STPLoader.Implementation.Model.Entity;
using STPLoader.Implementation.Parser;
using UnityEngine;
using System.Collections;

public class FileLoader : MonoBehaviour
{
    private string _path = @"C:\Users\squad\git\stp-loader\STPLoader\Example\bin\Debug\Gehaeuserumpf.stp";
    private STPLoader.ILoader _loader;
    private IParser _parser = new StpParser();
    
    void OnGUI()
    {
        _path = GUILayout.TextField(_path);

        if (GUILayout.Button("Load File"))
        {
            _loader = LoaderFactory.CreateFileLoader(_path);
            var result = _parser.Parse(_loader.Load());
            var coordinates = result.All<CartesianPoint>();
            foreach (var cartesianPoint in coordinates)
            {
                DrawPoint(cartesianPoint);
            }
        }    
    }

    private void DrawPoint(CartesianPoint cartesianPoint)
    {
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = ToUnity(cartesianPoint.Vector);
    }

    private Vector3 ToUnity(AForge.Math.Vector3 v3)
    {
        return new Vector3(v3.X, v3.Y, v3.Z);
    }
}
