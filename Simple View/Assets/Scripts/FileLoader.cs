using System.Collections.Generic;
using Assets.Scripts.Model;
using BasicLoader;
using CADLoader;
using UnityEngine;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

namespace Assets.Scripts
{
    public class FileLoader : MonoBehaviour
    {
        [SerializeField] private bool cfgSpawnedObject_EnablePhysics = true;

        [SerializeField] private bool cfgSpawnedObject_EnableGravity = false;

        [SerializeField] private float cfgSpawnedObject_Drag = 5.28f;

        [SerializeField] private bool cfgSpawnedObject_LockPosition = false;

        [SerializeField] private bool cfgSpawnedObject_LockRotation = true;

        [SerializeField] private string defaultPath =
            @"C:\Users\squad\Downloads\CAD_Daten_Lagertraeger\Gehaeuserumpf.stl";

        [SerializeField] private GameObject cfgFileNameField;

        [SerializeField] private GameObject cfgModelManager;

        private InputField _text;

        public void Start()
        {
            _text = cfgFileNameField.GetComponent<InputField>();
            _text.text = defaultPath;
        }

        public void LoadFile()
        {
            var loader =
                new CADLoader.CADLoader(new List<IParser>
                {
                    STPLoader.ParserFactory.Create(),
                    STLLoader.ParserFactory.Create(),
                    ThreeDXMLLoader.ParserFactory.Create()
                });
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
                //var meshCollider = go.AddComponent<MeshCollider>();
                //meshCollider.convex = true;
                go.transform.parent = baseObject.transform;
                go.transform.localPosition = Builder.ToUnity(part.Position);
            }

            if (cfgSpawnedObject_EnablePhysics)
            {
                var baseObjectRigidbody = baseObject.AddComponent<Rigidbody>();
                baseObjectRigidbody.angularDrag = cfgSpawnedObject_Drag;
                baseObjectRigidbody.drag = cfgSpawnedObject_Drag;
                baseObjectRigidbody.useGravity = cfgSpawnedObject_EnableGravity;

                RigidbodyConstraints constraints = RigidbodyConstraints.None;

                if (cfgSpawnedObject_LockPosition && cfgSpawnedObject_LockRotation)
                {
                    constraints = RigidbodyConstraints.FreezeAll;
                }
                else if (cfgSpawnedObject_LockPosition && !cfgSpawnedObject_LockRotation)
                {
                    constraints = RigidbodyConstraints.FreezePosition;
                }
                else if (!cfgSpawnedObject_LockPosition && cfgSpawnedObject_LockRotation)
                {
                    constraints = RigidbodyConstraints.FreezeRotation;
                }

                baseObjectRigidbody.constraints = constraints;
            }

            baseObject.AddComponent<ModelActions>();

            dataLoader.Close();
        }
    }
}