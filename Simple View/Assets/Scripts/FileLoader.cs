using System.Linq;
using STPConverter;
using STPLoader;
using STPLoader.Interface;
using UnityEngine;

namespace Assets.Scripts
{
    public class FileLoader : MonoBehaviour
    {
        private string _path = @"C:\Users\squad\git\stp-loader\STPLoader\Example\bin\Debug\Gehaeuserumpf.stp";
        private ILoader _loader;
        private IParser _parser = ParserFactory.Create();
        private IConverter _converter = ConverterFactory.Create();
    
        void OnGUI()
        {
            _path = GUILayout.TextField(_path);

            if (GUILayout.Button("Load File"))
            {
                _loader = LoaderFactory.CreateFileLoader(_path);
                var model = _parser.Parse(_loader.Load());
                var convertedModel = _converter.Convert(model);


                var modelObject = new GameObject("Model");

                var mesh = new Mesh();

                modelObject.transform.GetComponent<MeshFilter>();

                if (!modelObject.transform.GetComponent<MeshFilter>() || !modelObject.transform.GetComponent<MeshRenderer>())
                {
                    modelObject.transform.gameObject.AddComponent<MeshFilter>();
                    modelObject.transform.gameObject.AddComponent<MeshRenderer>();
                }

                modelObject.transform.GetComponent<MeshFilter>().mesh = mesh;


                var vertices = convertedModel.Points.Select<AForge.Math.Vector3,Vector3>(ToUnity).ToArray();
                var triangles = convertedModel.Triangles.ToArray();

                mesh.vertices = vertices;
                mesh.triangles = triangles;

                mesh.RecalculateNormals();
                mesh.Optimize();

                modelObject.GetComponent<MeshFilter>().mesh = mesh;
            }    
        }

    

        private static Vector3 ToUnity(AForge.Math.Vector3 v3)
        {
            return new Vector3(v3.X, v3.Z, v3.Y);
        }
    }
}
