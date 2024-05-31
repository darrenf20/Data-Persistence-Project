using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    [SerializeField] TMP_InputField nameInput;
    public string playerName;

    [SerializeField] TMP_Text highScoresText;
    [SerializeField] Text bestScore;
    private const int maxHighScores = 5;
    public string[] highScoreNames;
    public int[] highScoreValues;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            LoadHighScores();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeScene(int scene)
    {
        // If about to play, get the entered player name
        if (scene == 1)
        {
            playerName = nameInput.text;
        }

        SceneManager.LoadScene(scene);

        OnGameSceneLoad();
    }

    void OnGameSceneLoad()
    {
        bestScore = GameObject.Find("Canvas/Best Score Text").GetComponent<Text>();
        UpdateBestScore();
    }


    public void UpdateHighScores(int score)
    {
        if (playerName.Equals("") || playerName == null)
        {
            return;
        }

        for (int i = 0; i < maxHighScores; i++)
        {
            if (score > highScoreValues[i])
            {
                string nextName = playerName;
                int nextValue = score;

                // Move down existing scores to make room for new score
                for (int j = i; j < maxHighScores; j++)
                {
                    string tempName = highScoreNames[j];
                    int tempValue = highScoreValues[j];

                    highScoreNames[j] = nextName;
                    highScoreValues[j] = nextValue;

                    nextName = tempName;
                    nextValue = tempValue;
                }

                SaveHighScores();
                UpdateBestScore();
                break;
            }
        }
    }

    void UpdateBestScore()
    {
        if (!highScoreNames[0].Equals(""))
        {
            bestScore.text = "Best Score : " + highScoreNames[0] + " - " + highScoreValues[0];
        }
    }

    [System.Serializable]
    class SaveData
    {
        public string lastName;
        public string[] highScoreNames;
        public int[] highScoreValues;
    }

    public void SaveHighScores()
    {
        SaveData data = new SaveData();

        data.lastName = playerName;
        data.highScoreNames = highScoreNames;
        data.highScoreValues = highScoreValues;

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savedhighscores.json", json);
    }

    public void LoadHighScores()
    {
        string path = Application.persistentDataPath + "/savedhighscores.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            nameInput.text = data.lastName;
            highScoreNames = data.highScoreNames;
            highScoreValues = data.highScoreValues;
        }
        else
        {
            highScoreNames = new string[maxHighScores];
            highScoreValues = new int[maxHighScores];
        }

        highScoresText.text = "-High Scores-\n";
        for (int i = 0; i < maxHighScores; i++)
        {
            if (!highScoreNames[i].Equals("") && highScoreNames[i] != null)
            {
                highScoresText.text += highScoreNames[i] + " : " + highScoreValues[i] + "\n";
            }
        }
    }
}
