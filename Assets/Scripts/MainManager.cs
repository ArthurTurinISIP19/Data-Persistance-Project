using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance;
    private string _recordName = string.Empty;
    private int _recordScore;
    private string _bestScoreNameString;
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    private string _userName;
    private Text ScoreText;
    private Text _bestScoreText;
    private GameObject GameOverText;

    public bool _isGameScene = false;
    private bool m_Started = false;
    private bool m_GameOver = false;
    private int m_Points;
    

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        LoadData();
    }

    private void Update()
    {
        if (!m_Started && _isGameScene)
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
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(0);
                _isGameScene = false;
                m_Started = false; 
                m_GameOver = false;
            }
        }
    }
    public void WaitOneSec()
    {
        Instance._isGameScene = true;
        Invoke("InitVariable", 0.2f);
        Invoke("CreateLevel", 0.2f);
    }
    public void InitVariable()
    {
        Ball = GameObject.Find("/Paddle/Ball").GetComponent<Rigidbody>();
        ScoreText = GameObject.Find("CurrentScore").GetComponent<Text>();
        _bestScoreText = GameObject.Find("BestScore").GetComponent<Text>();
        GameOverText = GameObject.Find("/Canvas").GetComponent<Transform>().transform.GetChild(2).gameObject;
        DeathZone deathzone = GameObject.Find("DeathZone").GetComponent<DeathZone>();
        deathzone.Manager = this;
        LoadData();
    }
    public void CreateLevel()
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

        _bestScoreText.text = _bestScoreNameString;
    }

    public void SetUserName(string userName) => _userName = userName;

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        if (m_Points > _recordScore)
        {
            SaveData();
        }
        m_Points = 0;
        m_GameOver = true;
        GameOverText.SetActive(true);
    }

    public void SaveData()
    {
        StoredData data = new StoredData();
        data.RecordScore = m_Points;
        data.RecordName = _userName;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadData()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            StoredData data = JsonUtility.FromJson<StoredData>(json);

            _recordName = data.RecordName;
            _recordScore = data.RecordScore;
            _bestScoreNameString = $"Best Score: {_recordName} : {_recordScore}";
        }
        else
        {
            _bestScoreNameString = string.Empty;
            _recordName = string.Empty;
            _recordScore = 0;   
        }
    }
}
