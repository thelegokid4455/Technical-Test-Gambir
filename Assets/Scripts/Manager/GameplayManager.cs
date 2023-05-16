using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [Header("Enemies")]
    [SerializeField] bool useEnemies;
    public List<GameObject> allEnemies = new List<GameObject>();

    public static GameplayManager instance;
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.GameStart();
    }

    // Update is called once per frame
    void Update()
    {
        if(allEnemies.Count <= 0)
        {
            if(useEnemies)
                FinishGame();
        }
    }

    void FinishGame()
    {
        GameManager.instance.GameFinish();
        UIManager.instance.GameFinish();
    }
}
