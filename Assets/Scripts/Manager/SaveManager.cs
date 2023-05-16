using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    [SerializeField] bool saveGame;

    public static SaveManager instance;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("GameHasSave"))
        {
            LoadGame();
        }
    }

    public void SaveGame()
    {
        //save all high scores
        for (int i = 0; i < GameManager.instance.highScore.Length; i++)
        {

            PlayerPrefs.SetInt("ScoreLevel" + i, GameManager.instance.highScore[i]);
        }
        
        //checks
        PlayerPrefs.SetInt("GameHasSave", 1);
    }

    public void LoadGame()
    {
        //load all high scores
        for (int i = 0; i < GameManager.instance.highScore.Length; i++)
        {
            GameManager.instance.highScore[i] = PlayerPrefs.GetInt("ScoreLevel" + i, 0);

        }
    }

    [ContextMenu("Delete Save")]
    public void ResetSaveData()
    {
        PlayerPrefs.DeleteAll();
    }
    private void OnApplicationQuit()
    {
        if (saveGame)
            SaveGame();
    }
}
