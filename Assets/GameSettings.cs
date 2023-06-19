using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    static private GameSettings _game_settings;
    static public GameSettings Instance => _game_settings;
    public enum GameMode
    {
        BO1, //0
        BO3, //1
        BO5  //2
    }
    [SerializeField] private GameMode gameMode = GameMode.BO1;
    [SerializeField] private int _roundCount = 1;
    [SerializeField] private int _currRoundNumber = 1;
    [SerializeField] private List<ulong> _winnersList;

    private void Awake()
    {
        //DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (_game_settings == null)
        {
            _game_settings = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void increaseRounds()
    {
        if(gameMode == GameMode.BO5)
        {
            return;
        }
        gameMode++;
        setGameMode();
    }

    public void decreaseRounds()
    {
        if(gameMode == GameMode.BO1)
        {
            return;
        }
        gameMode--;
        setGameMode();
    }
    
    public void setGameMode()
    {
        _currRoundNumber = 1;
        if (gameMode == GameMode.BO1) _roundCount = 1;
        if (gameMode == GameMode.BO3) _roundCount = 3;
        if (gameMode == GameMode.BO5) _roundCount = 5;
        Debug.Log("Set Game Mode to: " + _roundCount + "\n");
    }

    public void increaseCurrRound()
    {
        _currRoundNumber++;
    }

    public int getCurrRound()
    {
        return _currRoundNumber;
    }

    public int getRoundCount()
    {
        return _roundCount;
    }

    public void addToWinnersList(ulong player)
    {
        _winnersList.Add(player);
    }

    public List<ulong> getFinalWinner()
    {
        return _winnersList.GroupBy(x => x).OrderByDescending(x => x.Count()).Select(x => x.Key).ToList();
    }
}
