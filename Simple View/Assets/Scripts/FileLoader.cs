using System.Collections.Generic;
using Assets.Scripts.Model;
using BasicLoader;
using CADLoader;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class FileLoader : MonoBehaviour
    {
        [SerializeField]
        private string defaultPath = @"C:\Users\squad\Downloads\CAD_Daten_Lagertraeger\Gehaeuserumpf.stl";

        [SerializeField]
        private GameObject cfgFileNameField;
        
        [SerializeField]
        private GameObject cfgModelManager;

        private InputField _text;

        public void Start()
        {
            _text = cfgFileNameField.GetComponent<InputField>();
            _text.text = defaultPath;
        }

        public void LoadFile()
        {
            var loader = new CADLoader.CADLoader(new List<IParser> { STPLoader.ParserFactory.Create(), STLLoader.ParserFactory.Create(), ThreeDXMLLoader.ParserFactory.Create() });
            var dataLoader = LoaderFactory.CreateFileLoader(_text.text);
            var type = CADTypeUtils.FromFileExtension(_text.text);
            var cadModel = loader.Load(type, dataLoader);
            var parts = cadModel.Parts;
            var baseObject = Builder.Create("Model");
            baseObject.transform.parent = cfgModelManager.transform;
            foreach (var part in parts)
            {
                var go = Builder.Create(part.Name, "defaultMat");
                Builder.UpdateMesh(ref go, part);
                var meshCollider = go.AddComponent<MeshCollider>();
                meshCollider.convex = true;
                go.transform.parent = baseObject.transform;
            }

            baseObject.AddComponent<Rigidbody>();
            
            dataLoader.Close();
        }
        
    }
}
