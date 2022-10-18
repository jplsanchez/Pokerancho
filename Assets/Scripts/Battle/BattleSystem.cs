using Controller;
using System;
using System.Collections;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit _playerUnit;
    [SerializeField] BattleUnit _foeUnit;
    [SerializeField] BattleDialogBox _dialogBox;
    [SerializeField] PartyScreen _partyScreen;

    PokemonParty _playerParty;
    PokemonParty _foePokemons;

    // Alternative: public event Action<bool> OnBattleOver;
    public delegate void BattleOverAction(bool won);
    public event BattleOverAction OnBattleOver;

    int _currentAction;
    int _currentMove;
    int _currentMember;

    bool _isBattleFinished = false;

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

        _partyScreen.Init();
        _dialogBox.SetMoveNames(_playerUnit.Pokemon.Moves);

        yield return _dialogBox.TypeDialog($"A wild {_foeUnit.Pokemon.Name} appeared");

        ActionSelection();
    }

    private void ClearEventsSubscribers()
    {
        ControllerManager.ButtonPressed -= HandleMoveSelection;
        ControllerManager.ButtonPressed -= HandleActionSelection;
        ControllerManager.ButtonPressed -= HandlePartySelection;
    }

    private void ActionSelection()
    {
        ClearEventsSubscribers();
        ControllerManager.ButtonPressed += HandleActionSelection;

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

    public void MoveSelection()
    {
        ClearEventsSubscribers();
        ControllerManager.ButtonPressed += HandleMoveSelection;

        _dialogBox.EnableActionSelector(false);
        _dialogBox.EnableDialogText(false);
        _dialogBox.EnableMoveSelector(true);

        _dialogBox.UpdateMoveSelection(_currentMove, _playerUnit.Pokemon.Moves[_currentMove]);
    }

    private void PerformMoveAnimation()
    {
        ClearEventsSubscribers();

        _dialogBox.EnableMoveSelector(false);
        _dialogBox.EnableDialogText(true);
        StartCoroutine(PlayerMove());
    }

    private IEnumerator PlayerMove()
    {
        yield return PerformMove(
            move: _playerUnit.Pokemon.Moves[_currentMove],
            sourceUnit: _playerUnit,
            targetUnit: _foeUnit);

        if(!_isBattleFinished) StartCoroutine(FoeMove());
    }

    private IEnumerator FoeMove()
    {
        yield return PerformMove(
            move: _foeUnit.Pokemon.GetRandomMove(),
            sourceUnit: _foeUnit,
            targetUnit: _playerUnit);

        if (!_isBattleFinished) ActionSelection();
    }

    private IEnumerator PerformMove(Move move, BattleUnit sourceUnit, BattleUnit targetUnit)
    {
        move.CurrentPP--;

        yield return _dialogBox.TypeDialog($"{sourceUnit.Pokemon.Name} used {move.Name}");
        sourceUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        targetUnit.PlayHitAnimation();
        var damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);
        yield return targetUnit.Hud.UpdateHp();

        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.isFainted)
        {
            yield return _dialogBox.TypeDialog($"{targetUnit.Pokemon.Name} Fainted");
            targetUnit.PlayFaintAnimation();

            yield return new WaitForSeconds(2f);


            CheckForBattleOver(targetUnit);


        }
    }

    private void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            if (!_playerParty.HasAnyHealthyPokemon()) BattleOver(false);

            OpenPartyScreen();
            return;
        }
        else
        {
            Pokemon nextPokemon = _foePokemons.GetFirstHealthyPokemon();
            if (nextPokemon is null) BattleOver(true);

            _foeUnit.Setup(nextPokemon);
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
                    MoveSelection();
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
                ActionSelection();
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
            StartCoroutine(SwitchPokemon(selectedMember));
        }
        else if (key == Key.B_Button)
        {
            _partyScreen.gameObject.SetActive(false);
            ActionSelection();
        }
        else _currentMember = MoveInMenu(key, _currentMember, 2, 2, numberOfItems: _playerParty.Pokemons.Count) ?? _currentMember;

        _partyScreen.UpdateMemberSelection(_currentMember);
    }

    private void BattleOver(bool won)
    {
        _isBattleFinished = true;

        OnBattleOver(won);
    }

    IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        if (_playerUnit.Pokemon.Hp > 0)
        {
            yield return _dialogBox.TypeDialog($"Come back {_playerUnit.Pokemon.Name}");
            _playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }

        _playerUnit.Setup(newPokemon);

        _dialogBox.SetMoveNames(newPokemon.Moves);

        _currentAction = 0;
        _currentMove = 0;
        _currentMember = 0;

        yield return _dialogBox.TypeDialog($"Go {newPokemon.Name}!");

        StartCoroutine(FoeMove());
    }
}
