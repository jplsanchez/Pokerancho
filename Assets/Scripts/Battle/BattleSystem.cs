using Controller;
using System;
using System.Collections;
using UnityEngine;

public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy, PartyScreen }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit _playerUnit;
    [SerializeField] BattleUnit _foeUnit;
    [SerializeField] BattleHud _playerHud;
    [SerializeField] BattleHud _foeHud;
    [SerializeField] BattleDialogBox _dialogBox;
    [SerializeField] PartyScreen _partyScreen;


    PokemonParty _playerParty;
    PokemonParty _foePokemons;

    // TODO 
    // Adicionar status de quem venceu a esse evento: Action<bool>
    public event Action OnBattleOver;

    BattleState _state;
    int _currentAction;
    int _currentMove;
    int _currentMember;

    public void StartBattle(PokemonParty playerParty, PokemonParty foeParty)
    {
        _playerParty = playerParty;
        _foePokemons = foeParty;

        StartCoroutine(SetupBattle());
    }

    private IEnumerator SetupBattle()
    {
        _playerUnit.Setup(_playerParty.GetFirstHealthyPokemon());
        _foeUnit.Setup(_foePokemons.GetAndRestoreRandomPokemon());
        _playerHud.SetData(_playerUnit.Pokemon);
        _foeHud.SetData(_foeUnit.Pokemon);

        _partyScreen.Init();

        _dialogBox.SetMoveNames(_playerUnit.Pokemon.Moves);

        yield return _dialogBox.TypeDialog($"A wild {_foeUnit.Pokemon.Name} appeared");

        PlayerAction();

    }

    private void ClearEventsSubscribers()
    {
        ControllerManager.ButtonPressed -= HandleMoveSelection;
        ControllerManager.ButtonPressed -= HandleActionSelection;
        ControllerManager.ButtonPressed -= HandlePartySelection;
    }

    private void PlayerAction()
    {
        ClearEventsSubscribers();
        ControllerManager.ButtonPressed += HandleActionSelection;

        _state = BattleState.PlayerAction;
        _dialogBox.SetDialog("Choose an action");
        _dialogBox.EnableDialogText(true);
        _dialogBox.EnableActionSelector(true);
        _dialogBox.EnableMoveSelector(false);

        _dialogBox.UpdateActionSelection(_currentAction);
    }

    private void OpenPartyScreen()
    {
        ClearEventsSubscribers();
        ControllerManager.ButtonPressed += HandlePartySelection;

        _state = BattleState.PartyScreen;
        _partyScreen.SetPartyData(_playerParty.Pokemons);
        _partyScreen.gameObject.SetActive(true);
        Debug.Log("Party Screen");
    }

    private void OpenBagScreen()
    {
        Debug.Log("Bag Screen");
    }

    private void RunAction()
    {
        Debug.Log("Run Action");
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
            enemyHud: _foeHud,
            party: _playerParty);

        if (_foeUnit.Pokemon.Hp > 0)
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
            enemyHud: _playerHud,
            party: _foePokemons);

        if (_playerUnit.Pokemon.Hp > 0)
        {
            PlayerAction();
        }
    }

    private IEnumerator PerformCharacterMove(Move move, BattleUnit charUnit, BattleUnit enemyUnit, BattleHud enemyHud, PokemonParty party)
    {
        move.CurrentPP--;

        yield return _dialogBox.TypeDialog($"{charUnit.Pokemon.Name} used {move.Name}");
        charUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        enemyUnit.PlayHitAnimation();
        var damageDetails = enemyUnit.Pokemon.TakeDamage(move, charUnit.Pokemon);
        yield return enemyHud.UpdateHp();

        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.isFainted)
        {
            yield return _dialogBox.TypeDialog($"{enemyUnit.Pokemon.Name} Fainted");
            enemyUnit.PlayFaintAnimation();

            yield return new WaitForSeconds(2f);

            var nextPokemon = party.GetFirstHealthyPokemon();
            if (nextPokemon != null )
            {
                // TODO fazer aqui
            }
            OnBattleOver();
        }
    }

    private IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f) yield return _dialogBox.TypeDialog("A critical hit!");

        if (damageDetails.TypeEffectiveness > 1f) yield return _dialogBox.TypeDialog("It's super effective!");
        else if (damageDetails.TypeEffectiveness < 1f) yield return _dialogBox.TypeDialog("It's not very effective!");

    }

    private static int? MoveInMenu(Key key, int currentPosition, int columns, int rows, int numberOfItems = 0)
    {
        if (numberOfItems == 0) numberOfItems = columns * rows;

        switch (key)
        {
            case Key.Right:
                if (currentPosition % rows < columns - 1) currentPosition++;
                break;

            case Key.Left:
                if (currentPosition % columns != 0) currentPosition--;
                break;

            case Key.Down:
                if (currentPosition + columns < numberOfItems) currentPosition += columns;
                break;

            case Key.Up:
                if (currentPosition - columns >= 0) currentPosition -= columns;
                break;

            default:
                return null;
        }

        return Mathf.Clamp(currentPosition, 0, numberOfItems - 1);

    }

    private void HandleActionSelection(Key key)
    {
        if (key == Key.A_Button) DoActionSelected();
        else _currentAction = MoveInMenu(key, _currentAction, 2, 2) ?? _currentAction;

        _dialogBox.UpdateActionSelection(_currentAction);

        void DoActionSelected()
        {
            switch (_currentAction)
            {
                case 0:
                    // Fight
                    PlayerMove();
                    break;
                case 1:
                    // Bag
                    OpenBagScreen();
                    break;
                case 2:
                    // Pokemon
                    OpenPartyScreen();
                    break;
                case 3:
                    // Run
                    RunAction();
                    break;
            }
        }
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

            case Key.B_Button:
                PlayerAction();
                break;

            default:
                return;
        }

        _dialogBox.UpdateMoveSelection(_currentMove, _playerUnit.Pokemon.Moves[_currentMove]);
    }

    private void HandlePartySelection(Key key)
    {
        if (key == Key.A_Button)
        {
            Pokemon selectedMember = _playerParty.Pokemons[_currentMember];

            if (selectedMember.Hp <= 0)
            {
                _partyScreen.SetMessageText("You can't send out a fainted pokemon");
                return;
            }
            if (selectedMember == _playerUnit.Pokemon)
            {
                _partyScreen.SetMessageText("You can't switch with the same pokemon");
                return;
            }

            ClearEventsSubscribers();
            _partyScreen.gameObject.SetActive(false);
            _dialogBox.EnableActionSelector(false);
            _state = BattleState.Busy;
            StartCoroutine(SwitchPokemon(selectedMember));
        }
        else if (key == Key.B_Button)
        {
            _partyScreen.gameObject.SetActive(false);
            PlayerAction();
        }
        else _currentMember = MoveInMenu(key, _currentMember, 2, 2, numberOfItems: _playerParty.Pokemons.Count) ?? _currentMember;

        _partyScreen.UpdateMemberSelection(_currentMember);
    }

    IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        yield return _dialogBox.TypeDialog($"Come back {_playerUnit.Pokemon.Name}");
        _playerUnit.PlayFaintAnimation();
        yield return new WaitForSeconds(2f);

        _playerUnit.Setup(newPokemon);
        _playerHud.SetData(newPokemon);

        _dialogBox.SetMoveNames(newPokemon.Moves);

        yield return _dialogBox.TypeDialog($"Go {newPokemon.Name}!");

        StartCoroutine(PerformFoeMove());
    }
}
