using System.Collections.Generic;
using Assets.Scripts.Model;
using BasicLoader;
using CADLoader;
using UnityEngine;

namespace Assets.Scripts
{
    public class FileLoader : MonoBehaviour
    {
        [SerializeField]
        private string defaultPath = @"C:\Users\squad\Downloads\CAD_Daten_Lagertraeger\Gehaeuserumpf.stl";
        
        public void OnClick()
        {
            var loader = new CADLoader.CADLoader(new List<IParser> { STPLoader.ParserFactory.Create(), STLLoader.ParserFactory.Create() });
            var dataLoader = LoaderFactory.CreateFileLoader(defaultPath);
            var type = CADTypeUtils.FromFileExtension(defaultPath);
            var cad_model = loader.Load(type, dataLoader);
            var models = cad_model.Models;
            var baseObject = Builder.Create("Model");
            foreach (var model in models)
            {
                var gameObject = Builder.Create(model.Name, "defaultMat");
                Builder.UpdateMesh(ref gameObject, model);
                gameObject.transform.parent = baseObject.transform;
            }
            dataLoader.Close();
        }
    }
}
