using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager instance;
    public string RecordHolder;
    public int CurrentRecordPoints;
    [SerializeField] private Text RecordText;

    public string path;

    
    private void Awake() //Responsible for "taking" the MainManager GameObj to the next scene
    {

        if (instance != null)
        {
            //Destroy(FindAnyObjectByType<SaveLoadManager>().gameObject);
            Destroy(gameObject);

            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        path = Application.persistentDataPath + "/savefile.json";

        LoadRecord();

        //Adds an event to update the UI reference when the scene changes
        SceneManager.sceneLoaded += OnSceneLoaded;

    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        //Updates the reference to Text whenever a new scene is loaded
        if (GameObject.Find("Record Text"))
        {
            RecordText = GameObject.Find("Record Text").GetComponent<Text>();
            RecordText.text = $"{RecordHolder.ToUpper()} has the current record whith {CurrentRecordPoints} points";
        }

    }

    private void OnDestroy()
    {
        //Remove the event when destroying the object to avoid errors
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        path = Application.persistentDataPath + "/savefile.json";

        if (File.Exists(path))
        {
            RecordText.text = $"{RecordHolder.ToUpper()} has the current record whith {CurrentRecordPoints} points";
        }
        else
        {
            RecordText.text = "";
        }       
    }


    [System.Serializable]
    class SaveData
    {
        public int RecordPoints; 
        public string Name; 
    }

    
    public void SaveRecord(string playerName, int m_Points) //Persists data between sessions
    {
        Debug.Log("SaveRecord() method has been called");

        SaveData data = new();

        data.Name = playerName;
        data.RecordPoints = m_Points;

        //Persists the fields of the class SaveData in the specified directory
        string json = JsonUtility.ToJson(data);   
        File.WriteAllText(path, json);

        //Informs that the data was saved and shows the directory
        Debug.Log("The data has been saved!");
        Debug.Log("Path to JSON file: " + path);   
    }

    //Recovers saved data
    public void LoadRecord()
    {

        if (File.Exists(path))
        {
            Debug.Log("Json file has been found!");

            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            //Assign the fields of the object created by JsonUtility to the global variable
            CurrentRecordPoints = data.RecordPoints;
            RecordHolder = data.Name;
        }
        else
        {
            Debug.Log("Json file hasn't been found");
            RecordText.text = "";
        }
    }

}
