using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema;
using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;
using TMPro;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Runtime.CompilerServices;
using UnityEngine.Events;
using UnityPhysics = UnityEngine.Physics;

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

        [SerializeField]
        private GameObject TaskParent;
        private GameObject[] Tasks;
        private int tasksFinished;
        
        //OnlyShowCurrentTask: Hides all other tasks, only shows the current one
        //ShowFinishedTasks: Shows the current one in white and all finished tasks in green
        public enum TaskDisplayMode { OnlyShowCurrentTask, ShowFinishedTasks};
        public TaskDisplayMode taskDisplayMode;

        public enum SpawnMethod { Instantiate, SetActive};
        public SpawnMethod spawnMethod;

        public GameObject Suzanne;

        //Tasks2
        Vector3 positionObject;

        //Task3
        Vector3 scaleObject;

        //Task4
        Quaternion rotationObject; // Ausrichtung

        //Starting Suzanne info
        Transform SuzanneStart;

        //Timer
        public float timeBetweenTasks = 3f;
        private float counter = 0;
        private bool resetValues = true;

        // [SerializeField]
        // [Tooltip("Scale factor to apply on load")]
        // private float ScaleFactor = 1.0f;

        private bool foundNewObject = false;
        private List<GameObject> spawnedObjects;

        public GameObject winningText;

        public TextMeshPro debugText;

        public void buttonPressToSimulate()
        {
            //The if statement only allows for one object. False if there is already an object.
            if (spawnedObjects.Count == 0)
                foundNewObject = true;
        }

        public void deleteObjects()
        {
            //delete all objects
            var allObjects = spawnedObjects.ToArray();
            for(int i = 0; i < allObjects.Length; i++)
            {
                Destroy(allObjects[i]);
            }
            spawnedObjects.Clear();

            //If all tasks were done, show winning text :)
            if (tasksFinished == Tasks.Length - 1)
                winningText.SetActive(true);

            //reset task counter
            tasksFinished = 0;

            //Reset all task texts
            Tasks[0].SetActive(true);
            for (int i = 1; i < Tasks.Length; i++)
            {
                Tasks[i].SetActive(false);
                Tasks[i].GetComponent<TextMeshPro>().color = Color.white;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            spawnedObjects = new List<GameObject>();
            //Count the number of tasks by counting the children of the Task GameObject (Interface)
            int numberOfTasks = TaskParent.transform.childCount;
            //Create new Tasks array
            Tasks = new GameObject[numberOfTasks];
            //Give each Tasks a reference to the corresponding child
            for (int i = 0; i < numberOfTasks; i++)
            {
                Tasks[i] = TaskParent.transform.GetChild(i).gameObject;
                if (i != 0)                         // Checkboxes have to be set to false, otherwise they appear when starting the game (except first one, index 0)
                    Tasks[i].SetActive(false);
            }
            winningText.SetActive(false);
            tasksFinished = 0;

            //Get starting transform from Suzanne
            SuzanneStart = Suzanne.GetComponent<Microsoft.MixedReality.Toolkit.UI.BoundingBox>().Target.transform;
        }

        // Update is called once per frame
        void Update()
        {
            //counter keeps counting up
            counter += Time.deltaTime;
            //gets the current transform of the object
           if (counter >= timeBetweenTasks && spawnedObjects.Count > 0 && resetValues == false)
            {
                resetValues = true;
                positionObject = spawnedObjects[0].GetComponent<Microsoft.MixedReality.Toolkit.UI.BoundingBox>().Target.transform.position;
                rotationObject = spawnedObjects[0].GetComponent<Microsoft.MixedReality.Toolkit.UI.BoundingBox>().Target.transform.rotation;
                scaleObject = spawnedObjects[0].GetComponent<Microsoft.MixedReality.Toolkit.UI.BoundingBox>().Target.transform.localScale;
            } 

            // scan umgebung
            // TODO

            // sobald Objekt gefunden, erkenne welches Objekt (ObjecrEnum)
            // TODO
            // foundNewObject = true;

            // Sobald Objekt erkannt wird, soll virtuelles Objekt geladen und platziert werden
            if (foundNewObject || UnityEngine.Input.GetButtonDown("debug"))
            {
                foundNewObject = false; // entering loop just wants, no duplication
                var whichObject = ObjectEnum.Block;

                //if winning text is still displayed
                winningText.SetActive(false);

                switch (whichObject)
                {
                    case ObjectEnum.Block:
                        // SpawnNewObject(blockRelativePath);
                        if (spawnMethod == SpawnMethod.Instantiate)
                            SpawnObject(Block);
                        else
                            Suzanne.SetActive(true);
                        //When spawning new Block, disable task 1 and enable task 2. (Only when task 1 hasn't been done yet)
                        if (tasksFinished == 0)
                        {
                            //Set the new task text displays method
                            DisplayNextTask();

                            //task counter goes one up
                            tasksFinished++;
                            counter = 0;


                            //Save position of the Object (old)
                            if (spawnMethod == SpawnMethod.Instantiate)
                                positionObject = spawnedObjects[0].GetComponent<Microsoft.MixedReality.Toolkit.UI.BoundingBox>().Target.transform.position; //before moving task, check where it is placed
                            else
                                positionObject = Suzanne.GetComponent<Microsoft.MixedReality.Toolkit.UI.BoundingBox>().Target.transform.position;
                        }
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

            /*
             * Task 2: Grab/Move object
             */
                                        // spawnedObject = real time
            if (counter >= timeBetweenTasks && tasksFinished == 1 && spawnedObjects[0].GetComponent<Microsoft.MixedReality.Toolkit.UI.BoundingBox>().Target.transform.position != positionObject /* Suzanne.GetComponent<Microsoft.MixedReality.Toolkit.UI.BoundingBox>().Target.transform.position != positionObject*/)
            {
                //What happens when the object's position has changed

                DisplayNextTask();

                //Save current rotation, in case it changed (old)
                if (spawnMethod == SpawnMethod.Instantiate)
                    rotationObject = spawnedObjects[0].GetComponent<Microsoft.MixedReality.Toolkit.UI.BoundingBox>().Target.transform.rotation;
                else
                    rotationObject = Suzanne.GetComponent<Microsoft.MixedReality.Toolkit.UI.BoundingBox>().Target.transform.rotation;

                //Increment task counter
                tasksFinished++;
                counter = 0;
                resetValues = false;
            }
            /*
             * Task 3: Rotate Object
             */
            else if (counter >= timeBetweenTasks && tasksFinished == 2 && spawnedObjects[0].GetComponent<Microsoft.MixedReality.Toolkit.UI.BoundingBox>().Target.transform.rotation != rotationObject /* Suzanne.GetComponent<Microsoft.MixedReality.Toolkit.UI.BoundingBox>().Target.transform.rotation != rotationObject*/)
            {
                //What happens when object's position has changed

                DisplayNextTask();

                //Save current localScale (old)
                if (spawnMethod == SpawnMethod.Instantiate)
                    scaleObject = spawnedObjects[0].GetComponent<Microsoft.MixedReality.Toolkit.UI.BoundingBox>().Target.transform.localScale;
                else
                    scaleObject = Suzanne.GetComponent<Microsoft.MixedReality.Toolkit.UI.BoundingBox>().Target.transform.localScale;

                //Increment task counter
                tasksFinished++;
                counter = 0;
                resetValues = false;
            }
            /*
             * Task 4: Resize Object
             */
            else if (counter >= timeBetweenTasks && tasksFinished == 3 && spawnedObjects[0].GetComponent<Microsoft.MixedReality.Toolkit.UI.BoundingBox>().Target.transform.localScale != scaleObject /* Suzanne.GetComponent<Microsoft.MixedReality.Toolkit.UI.BoundingBox>().Target.transform.localScale != scaleObject*/)
            {
                //What happens when object's scale has changed

                DisplayNextTask();

                //Increment task counter
                tasksFinished++;
            }

           

            //Debug
            /*
            if(spawnMethod == SpawnMethod.Instantiate)
                debugText.text = "Meshpos: " + spawnedObjects[0].transform.position + " | Saved position: " + positionObject + Time.time;
            else
                debugText.text = "Meshpos: " + Suzanne.GetComponent<Microsoft.MixedReality.Toolkit.UI.BoundingBox>().Target.transform.position + " | Saved position: " + positionObject + Time.time;
            */
        }

        private void DisplayNextTask()
        {
            if (taskDisplayMode == TaskDisplayMode.OnlyShowCurrentTask)
            {
                Tasks[tasksFinished].SetActive(false);  //Hide old task
                Tasks[tasksFinished + 1].SetActive(true); //Show new task
            }
            else if (taskDisplayMode == TaskDisplayMode.ShowFinishedTasks)
            {
                Tasks[tasksFinished].GetComponent<TextMeshPro>().color = Color.green;   //Color task green
                Tasks[tasksFinished + 1].SetActive(true);                               //Show next task
            }
        }

        private void SpawnObject(GameObject prefabToSpawn)
        {
            //Get position between the two button texts, so it will spawn right between them.
            Vector3 positionToSpawn = (Tasks[0].transform.position + Tasks[Tasks.Length - 1].transform.position) / 2f;
            Quaternion rotation = Quaternion.identity;
            var newObj = Instantiate(prefabToSpawn, positionToSpawn, rotation);
            spawnedObjects.Add(newObj);
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