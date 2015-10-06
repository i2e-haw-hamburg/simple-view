using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CADLoader;
using UnityEngine;

namespace Assets.Scripts.Model
{
    static class Builder
    {
        public static GameObject Create(string name, string materialName)
        {
            var modelObject = new GameObject(name);


            modelObject.transform.GetComponent<MeshFilter>();

            if (!modelObject.transform.GetComponent<MeshFilter>() || !modelObject.transform.GetComponent<MeshRenderer>())
            {
                modelObject.transform.gameObject.AddComponent<MeshFilter>();
                modelObject.transform.gameObject.AddComponent<MeshRenderer>();
            }

            var material = Resources.Load<Material>(string.Format("{0}", materialName));
            Debug.Log(material);
            modelObject.transform.GetComponent<MeshRenderer>().material = material;

            var mesh = new Mesh();
            modelObject.transform.GetComponent<MeshFilter>().mesh = mesh;

            return modelObject;
        }

        public static void UpdateMesh(GameObject gameObject, IModel model)
        {
            var mesh = gameObject.transform.GetComponent<MeshFilter>().mesh;

            mesh.vertices = model.Vertices.Select<AForge.Math.Vector3, Vector3>(ToUnity).ToArray();
            mesh.triangles = model.Triangles.ToArray();

            mesh.RecalculateNormals();
            mesh.Optimize();

            gameObject.GetComponent<MeshFilter>().mesh = mesh;
        }

        private static Vector3 ToUnity(AForge.Math.Vector3 v3)
        {
            return new Vector3(v3.X, v3.Z, v3.Y);
        }
    }
}
