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

    int currentAction;
    int currentMove;

    private void Start()
    {
        StartCoroutine(SetupBattle());
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

    private void PlayerAction()
    {
        state = BattleState.PlayerAction;
        StartCoroutine(dialogBox.TypeDialog("Choose an action"));
        dialogBox.EnableActionSelector(true);
    }

    public void PlayerMove()
    {
        state = BattleState.PlayerMove;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    private void Update()
    {
        if (state == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }
        else
        if (state == BattleState.PlayerMove)
        {
            HandleMoveSelection();
        }
    }


    private void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentAction < 1) ++currentAction;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAction > 0) --currentAction;
        }

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0) PlayerMove();
        }

        //dialogBox.CheckClickOnAction();
        //if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && dialogBox.)
        //{
        //    --score;
        //}


        //if (Input.GetMouseButton(0))
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit;
        //    // Casts the ray and get the first game object hit
        //    Physics.Raycast(ray, out hit);
        //    Debug.Log("This hit at " + hit.point);
        //}
    }

    private void HandleMoveSelection()
    {
        int numberOfMoves = playerUnit.Pokemon.Moves.Count;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentMove < numberOfMoves-  1) ++currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentMove > 0) --currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentMove < numberOfMoves - 2) currentMove +=  2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMove > 1) currentMove -= 2;
        }

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

        //if (Input.GetKeyDown(KeyCode.Z))
        //{
        //    if (currentMove == 0) PlayerMove();
        //}
    }
}
