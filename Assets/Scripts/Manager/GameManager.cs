using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game")]
    public bool hasStarted;
    public bool isPaused;
    public int selectedLevel;

    [Header("Global")]

    public bool globalIsLoading;
    public float globalMusicVolume;
    public float globalEffectVolume;

    [Header("Sound")]
    public AudioClip soundSelected;

    [Header("Score")]
    public int scoreKillZombie;
    public int scoreKillTurret;
    public int scoreGetPickup;
    public int scoreDestroyMine;

    [Header("Player Data")]
    public int currentScore;
    public int[] highScore;


    public static GameManager instance;
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        ResetData();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPaused)
        {
            GameSetTimeScale(0);
        }
        else
        {
            GameSetTimeScale(1);
        }
    }

    public void PlayAudio(AudioClip clip)
    {
        GetComponent<AudioSource>().PlayOneShot(clip, globalEffectVolume/100);
    }

    public void GameSetTimeScale(float value)
    {
        Time.timeScale = value;
    }


    public void GameStart()
    {
        hasStarted = true;
        print("Start");
    }

    public void GameFinish()
    {
        hasStarted = false;
        print("Finish");

        if (currentScore > highScore[selectedLevel])
            highScore[selectedLevel] = currentScore;
    }

    public void ResetData()
    {
        hasStarted = false;
        GameSetTimeScale(1);
    }

    //score
    public void AddScore(int amount)
    {
        currentScore += amount;
    }

    public void ResetScore()
    {
        currentScore = 0;
    }
}