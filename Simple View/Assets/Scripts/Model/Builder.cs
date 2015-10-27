using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicLoader;
using BasicLoader.Interface;
using CADLoader;
using UnityEngine;

namespace Assets.Scripts.Model
{
    static class Builder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="materialName"></param>
        /// <returns></returns>
        public static GameObject Create(string name, string materialName=null)
        {
            var modelObject = new GameObject(name);
            if (materialName != null)
            {
                modelObject.transform.GetComponent<MeshFilter>();
                if (!modelObject.transform.GetComponent<MeshFilter>() || !modelObject.transform.GetComponent<MeshRenderer>())
                {
                    modelObject.transform.gameObject.AddComponent<MeshFilter>();
                    modelObject.transform.gameObject.AddComponent<MeshRenderer>();
                }
                var material = Resources.Load<Material>(string.Format("{0}", materialName));
                modelObject.transform.GetComponent<MeshRenderer>().material = material;
                var mesh = new Mesh();
                modelObject.transform.GetComponent<MeshFilter>().mesh = mesh;
            }
            return modelObject;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="part"></param>
        public static void UpdateMesh(ref GameObject gameObject, IPart part)
        {
            var mesh = gameObject.transform.GetComponent<MeshFilter>().mesh;

            mesh.vertices = part.Vertices.Select<AForge.Math.Vector3, Vector3>(ToUnity).ToArray();
            mesh.triangles = part.Triangles.ToArray();

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
