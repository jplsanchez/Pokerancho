using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] BattleSystem _battleSystem;
    [SerializeField] Camera _menuCamera;

    public void StartBattle()
    {
        _battleSystem.gameObject.SetActive(true);
        _menuCamera.gameObject.SetActive(false);

        _battleSystem.StartBattle();

        _battleSystem.OnBattleOver += EndBattle;
    }

    public void EndBattle()
    {
        _menuCamera.gameObject.SetActive(true);
        _battleSystem.gameObject.SetActive(false);

        _battleSystem.OnBattleOver -= EndBattle;
    }
}
