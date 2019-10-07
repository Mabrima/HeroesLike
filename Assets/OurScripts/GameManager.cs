using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public UnitHandler[] unitsInCombat;
    public UnitHandler currentUnit;

    public enum GameState
    {
        CombatMovement, CombatAttack, 
    }

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        GameHandler();
    }

    public void GameHandler()
    {

    }

}
