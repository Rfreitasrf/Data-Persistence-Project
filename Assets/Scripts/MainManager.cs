using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public Rigidbody Ball;
    public Text ScoreText;
    public Text BestScoreText;
    public TMP_InputField InputField; //Allow access to typed text
    public GameObject NameField; //Text Box
    public GameObject GameOverText1;
    public GameObject GameOverText2;

    private bool m_Started = false;
    private bool m_GameOver = false;
    public int LineCount = 6;
    private int m_Points;
    private string m_RecordHolder;

    //Receives an instance of LoadSaveManager when the scene is loaded
    public int m_PointsRecord; 
    public bool NewRecord;

    //Start is called before the first frame update
    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }

        //Listener calls the method after the user types the name when the record is broken
        InputField.onEndEdit.AddListener(OnNameEntered);

        //Access the values ​​in the SaveLoadManager class
        m_PointsRecord = SaveLoadManager.instance.CurrentRecordPoints;
        m_RecordHolder = SaveLoadManager.instance.RecordHolder;

        //Ensures that the text will only appear if there is a Json file
        if (File.Exists(SaveLoadManager.instance.path))
        {
            BestScoreText.text = $"Best Score - {m_RecordHolder}: {m_PointsRecord} Points";
        }
        else
        {
            BestScoreText.text = "";
        }

        //Initializes the variable responsible for verify if the record were broken
        NewRecord = false;
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver && !NewRecord)
        {
            //warning about pressing space to restart
            GameOverText1.SetActive(true);

            //This option is only available if there the record weren't broken
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;
        
        //If the record is broken do
        if (m_PointsRecord < m_Points)
        {
            //This informs that the record was broken.
            GameOverText2.SetActive(true);

            //Reveals text box for player to write name
            NameField.SetActive(true);
            InputField.text = "";
            InputField.Select();

            NewRecord = true;
        }
    }

    //Called by the Event listener when the player finishes typing the name
    private void OnNameEntered(string playerName)
    {
        if (!string.IsNullOrEmpty(playerName))
        {
            SaveLoadManager.instance.SaveRecord(playerName, m_Points);//Call the save method

            //Call the method that restart the scene
            if (m_GameOver && NewRecord)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

                SaveLoadManager.instance.LoadRecord();
            }
        }
        else
        {
            Debug.LogWarning("The name field cannot be empty! Please type it again.");
            InputField.Select(); //Keep focus on the input field
        }
    }
}
