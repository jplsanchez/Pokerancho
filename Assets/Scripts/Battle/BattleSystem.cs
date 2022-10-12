using Controller;
using System;
using System.Collections;
using UnityEngine;

public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit foeUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleHud foeHud;
    [SerializeField] BattleDialogBox dialogBox;

    BattleState state;
    BattleState controllerState;

    int currentAction;
    int currentMove;

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
        playerUnit.Setup();
        foeUnit.Setup();
        playerHud.SetData(playerUnit.Pokemon);
        foeHud.SetData(foeUnit.Pokemon);

        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);

        yield return dialogBox.TypeDialog($"A wild {foeUnit.Pokemon.Name} appeared");
        yield return new WaitForSeconds(1f);

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

        state = BattleState.PlayerAction;
        StartCoroutine(dialogBox.TypeDialog("Choose an action"));
        dialogBox.EnableActionSelector(true);
    }

    public void PlayerMove()
    {
        ClearEventsSubscribers();
        ControllerManager.ButtonPressed += HandleMoveSelection;

        state = BattleState.PlayerMove;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    private void HandleActionSelection(Key key)
    {
        switch (key)
        {
            case Key.Down:
                if (currentAction < 1) ++currentAction;
                break;

            case Key.Up:
                if (currentAction > 0) --currentAction;
                break;

            case Key.A_Button:
                if (currentAction == 0) PlayerMove();
                break;

            default:
                return;
        }

        dialogBox.UpdateActionSelection(currentAction);
    }

    private void HandleMoveSelection(Key key)
    {
        int numberOfMoves = playerUnit.Pokemon.Moves.Count;

        switch (key)
        {
            case Key.Right:
                if (currentMove < numberOfMoves - 1) ++currentMove;
                break;

            case Key.Left:
                if (currentMove > 0) --currentMove;
                break;

            case Key.Down:
                if (currentMove < numberOfMoves - 2) currentMove += 2;
                break;

            case Key.Up:
                if (currentMove > 1) currentMove -= 2;
                break;

            default:
                return;
        }

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);
    }
}
