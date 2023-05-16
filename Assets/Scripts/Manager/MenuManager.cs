using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class MenuManager : MonoBehaviour
{
    //loading
    GameObject fadeObject;

    [Header("Menu")]
    [SerializeField] private bool isMenu;
    [SerializeField] private bool canPause;

    [Header("Pause")]
    [SerializeField] GameObject pauseMenu;

    [Header("Score Text")]
    [SerializeField] Text[] levelScoreText;

    public static MenuManager instance;
    private void Awake()
    {
        instance = this;
        if (!GameManager.instance)
            SceneManager.LoadScene(0);
    }

    // Start is called before the first frame update
    void Start()
    {
        fadeObject = GameObject.FindGameObjectWithTag("Fade");

        if (isMenu)
        {

        }
        else
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        GameManager.instance.globalIsLoading = fadeObject.GetComponent<Animation>().isPlaying;
        if (isMenu)
        {
            for (int i = 0; i < GameManager.instance.highScore.Length; i++)
            {
                levelScoreText[i].text = "Score: " + GameManager.instance.highScore[i];
            }
        }
        else
        {
            if(GameManager.instance.hasStarted)
            {
                if(canPause)
                {
                    if (Input.GetButtonDown("Pause"))
                    {
                        if (!GameManager.instance.isPaused)
                        {
                            if (GameManager.instance.globalIsLoading)
                                return;
                            GamePause();
                        }
                        else
                            GameUnPause();
                    }
                }
                

            }
            else
            {


            }

        }
    }

    //timer
    public static string TimeFormatter(float seconds, bool forceHHMMSS = false)
    {
        float secondsRemainder = Mathf.Floor((seconds % 60) * 100) / 100.0f;
        int minutes = ((int)(seconds / 60)) % 60;
        int hours = (int)(seconds / 3600);

        if (!forceHHMMSS)
        {
            if (hours == 0)
            {
                return System.String.Format("{0:00}:{1:00.00}", minutes, secondsRemainder);
            }
        }
        return System.String.Format("{0}:{1:00}:{2:00}", hours, minutes, secondsRemainder);
    }


    //audio
    public void ButtonPlayAudio(AudioClip clip)
    {
        GameManager.instance.PlayAudio(clip);
    }
    public void ButtonPlaySelectAudio()
    {
        GameManager.instance.PlayAudio(GameManager.instance.soundSelected);
    }

    //menu
    public void GameRestartLevel()
    {
        if (GameManager.instance.globalIsLoading)
            return;

        GameUnPause();
        GameLoadLevel(SceneManager.GetActiveScene().name);
    }

    public void GameLoadLevel(string level)
    {
        if (GameManager.instance.globalIsLoading)
            return;

        GameUnPause();
        StartCoroutine(LoadingLevel(level));
        
    }

    IEnumerator LoadingLevel(string level)
    {
        //fade
        fadeObject.GetComponent<Animation>().Play("FadeToBlack");
        //music
        yield return new WaitForSeconds(fadeObject.GetComponent<Animation>().clip.length);
        SceneManager.LoadScene(level);
    }

    //gameplay
    public void GameStart()
    {
        if (GameManager.instance)
        {
            GameManager.instance.GameStart();
        }
    }
    public void GameFinish()
    {
        if (GameManager.instance)
        {
            GameManager.instance.GameFinish();
        }
    }
    public void GamePause()
    {
        GameManager.instance.isPaused = true;
        if (pauseMenu)
            pauseMenu.SetActive(true);
    }
    public void GameUnPause()
    {
        GameManager.instance.isPaused = false;
        if(pauseMenu)
            pauseMenu.SetActive(false);
    }


    //application
    public void GameQuit()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    //Level
    public void MenuSelectLevel(int value)
    {
        GameManager.instance.selectedLevel = value;
    }
}