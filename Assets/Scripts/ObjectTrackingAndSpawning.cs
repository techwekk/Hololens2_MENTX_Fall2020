using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema;
using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Serialization;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.Gltf
{

    public class ObjectTrackingAndSpawning : MonoBehaviour
    {
        /*
        [SerializeField]
        [FormerlySerializedAs("uri")]
        [Tooltip("The relative asset path to the glTF asset in the Streaming Assets folder.")]
        private string blockRelativePath = "GltfModels/Lantern/glTF/Lantern.gltf";

        [SerializeField]
        [FormerlySerializedAs("uri")]
        [Tooltip("The relative asset path to the glTF asset in the Streaming Assets folder.")]
        private string cupRelativePath = "GltfModels/Lantern/glTF/Lantern.gltf";
        */
        [SerializeField]
        private GameObject Block;

        [SerializeField]
        private GameObject Cup;

        // [SerializeField]
        // [Tooltip("Scale factor to apply on load")]
        // private float ScaleFactor = 1.0f;

        private bool foundNewObject = false;

        public void buttonPressToSimulate()
        {
            foundNewObject = true;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            // scan umgebung
            // TODO

            // sobald Objekt gefunden, erkenne welches Objekt (ObjecrEnum)
            // TODO
            // foundNewObject = true;

            // Sobald Objekt erkannt wird, soll virtuelles Objekt geladen und platziert werden
            if (foundNewObject || UnityEngine.Input.GetButtonDown("debug"))
            {
                foundNewObject = false;
                var whichObject = ObjectEnum.Block;

                switch (whichObject)
                {
                    case ObjectEnum.Block:
                        // SpawnNewObject(blockRelativePath);
                        SpawnObject(Block);
                        break;
                    case ObjectEnum.Cup:
                        // SpawnNewObject(cupRelativePath);
                        SpawnObject(Cup);
                        break;
                    default:
                        //
                        break;
                }
            }
        }

        private void SpawnObject(GameObject prefabToSpawn)
        {
            Vector3 positionToSpawn = Vector3.zero;
            Quaternion rotation = Quaternion.identity;
            Instantiate(prefabToSpawn, positionToSpawn, rotation);
        }

        /*
        private async void SpawnNewObject(string pathToObject)
        {
            var objectPath = Path.Combine(Path.GetFullPath(Application.streamingAssetsPath), pathToObject.NormalizeSeparators());
            if (!File.Exists(objectPath))
            {
                Debug.LogError($"Unable to find the glTF object at {objectPath}");
                return;
            }


            GltfObject gltfObject = null;

            try
            {
                gltfObject = await GltfUtility.ImportGltfObjectFromPathAsync(objectPath);

                // Put object in front of user
                gltfObject.GameObjectReference.transform.position = new Vector3(0.0f, 0.0f, 1.0f);

                gltfObject.GameObjectReference.transform.localScale *= this.ScaleFactor;
            }
            catch (Exception e)
            {
                Debug.LogError($"TestGltfLoading start failed - {e.Message}\n{e.StackTrace}");
            }

            if (gltfObject != null)
            {
                Debug.Log("Import successful");
            }
        }
        */
    }
}