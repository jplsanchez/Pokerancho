using Controller;
using System;
using System.Collections;
using UnityEngine;

public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit _playerUnit;
    [SerializeField] BattleUnit _foeUnit;
    [SerializeField] BattleHud _playerHud;
    [SerializeField] BattleHud _foeHud;
    [SerializeField] BattleDialogBox _dialogBox;

    BattleState _state;

    int _currentAction;
    int _currentMove;

    private void Start()
    {
        StartCoroutine(SetupBattle());
        ControllerManager.ButtonPressed += Test;
    }

    // TEST
    private void Test(Key message)
    {
        Debug.Log($"Clicou no botão {Enum.GetName(typeof(Key), message)}");
    }

    private IEnumerator SetupBattle()
    {
        _playerUnit.Setup();
        _foeUnit.Setup();
        _playerHud.SetData(_playerUnit.Pokemon);
        _foeHud.SetData(_foeUnit.Pokemon);

        _dialogBox.SetMoveNames(_playerUnit.Pokemon.Moves);

        yield return _dialogBox.TypeDialog($"A wild {_foeUnit.Pokemon.Name} appeared");

        PlayerAction();

    }

    private void ClearEventsSubscribers()
    {
        ControllerManager.ButtonPressed -= HandleMoveSelection;
        ControllerManager.ButtonPressed -= HandleActionSelection;
    }

    private void PlayerAction()
    {
        ClearEventsSubscribers();
        ControllerManager.ButtonPressed += HandleActionSelection;

        _state = BattleState.PlayerAction;
        StartCoroutine(_dialogBox.TypeDialog("Choose an action"));
        _dialogBox.EnableActionSelector(true);

        _dialogBox.UpdateActionSelection(_currentAction);
    }

    public void PlayerMove()
    {
        ClearEventsSubscribers();
        ControllerManager.ButtonPressed += HandleMoveSelection;

        _state = BattleState.PlayerMove;
        _dialogBox.EnableActionSelector(false);
        _dialogBox.EnableDialogText(false);
        _dialogBox.EnableMoveSelector(true);

        _dialogBox.UpdateMoveSelection(_currentMove, _playerUnit.Pokemon.Moves[_currentMove]);
    }

    private void PerformMoveAnimation()
    {
        ClearEventsSubscribers();

        _state = BattleState.Busy;
        _dialogBox.EnableMoveSelector(false);
        _dialogBox.EnableDialogText(true);
        StartCoroutine(PerformPlayerMove());
    }

    private IEnumerator PerformPlayerMove()
    {
        yield return PerformCharacterMove(
            move: _playerUnit.Pokemon.Moves[_currentMove], 
            charUnit: _playerUnit, 
            enemyUnit: _foeUnit, 
            enemyHud: _foeHud);

        if(_foeUnit.Pokemon.Hp > 0)
        {
            StartCoroutine(PerformFoeMove());
        }
    }    

    private IEnumerator PerformFoeMove()
    {
        _state = BattleState.EnemyMove;

        yield return PerformCharacterMove(
            move: _foeUnit.Pokemon.GetRandomMove(), 
            charUnit: _foeUnit, 
            enemyUnit: _playerUnit, 
            enemyHud: _playerHud);

        if(_playerUnit.Pokemon.Hp > 0)
        {
            PlayerAction();
        }
    }

    private IEnumerator PerformCharacterMove(Move move, BattleUnit charUnit, BattleUnit enemyUnit, BattleHud enemyHud)
    {
        yield return _dialogBox.TypeDialog($"{charUnit.Pokemon.Name} used {move.Name}");
        charUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        enemyUnit.PlayHitAnimation();
        var damageDetails = enemyUnit.Pokemon.TakeDamage(move, charUnit.Pokemon);
        yield return enemyHud.UpdateHp();

        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.isFainted)
        {
            yield return _dialogBox.TypeDialog($"{enemyUnit.Pokemon.Name} Fanted");
            enemyUnit.PlayFaintAnimation();
        }
    }

    private IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f) yield return _dialogBox.TypeDialog("A critical hit!");

        if (damageDetails.TypeEffectiveness > 1f) yield return _dialogBox.TypeDialog("It's super effective!");
        else if (damageDetails.TypeEffectiveness < 1f) yield return _dialogBox.TypeDialog("It's not very effective!");

    }

    private void HandleActionSelection(Key key)
    {
        switch (key)
        {
            case Key.Down:
                if (_currentAction < 1) ++_currentAction;
                break;

            case Key.Up:
                if (_currentAction > 0) --_currentAction;
                break;

            case Key.A_Button:
                if (_currentAction == 0) PlayerMove();
                break;

            default:
                return;
        }

        _dialogBox.UpdateActionSelection(_currentAction);
    }

    private void HandleMoveSelection(Key key)
    {
        int numberOfMoves = _playerUnit.Pokemon.Moves.Count;

        switch (key)
        {
            case Key.Right:
                if (_currentMove < numberOfMoves - 1) ++_currentMove;
                break;

            case Key.Left:
                if (_currentMove > 0) --_currentMove;
                break;

            case Key.Down:
                if (_currentMove < numberOfMoves - 2) _currentMove += 2;
                break;

            case Key.Up:
                if (_currentMove > 1) _currentMove -= 2;
                break;

            case Key.A_Button:
                PerformMoveAnimation();
                break;

            default:
                return;
        }

        _dialogBox.UpdateMoveSelection(_currentMove, _playerUnit.Pokemon.Moves[_currentMove]);
    }

}
