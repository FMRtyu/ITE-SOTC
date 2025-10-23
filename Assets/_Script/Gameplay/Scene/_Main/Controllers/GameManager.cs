using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameState CurrentState { get; private set; }

    public UIManager uIManager;

    void Start()
    {
        
    }
}
