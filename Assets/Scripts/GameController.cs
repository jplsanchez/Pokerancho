using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] BattleSystem _battleSystem;
    [SerializeField] Menu _menu;

    public void StartBattle()
    {
        _battleSystem.gameObject.SetActive(true);
        _menu.gameObject.SetActive(false);

        _battleSystem.StartBattle(_menu.PlayerParty, _menu.FoeParty);

        _battleSystem.OnBattleOver += EndBattle;
    }

    public void EndBattle()
    {
        _menu.gameObject.SetActive(true);
        _battleSystem.gameObject.SetActive(false);

        _battleSystem.OnBattleOver -= EndBattle;
    }
}
