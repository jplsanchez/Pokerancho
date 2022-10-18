using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField]Text messageText;

    PartyMemberUI[] _memberSlots;
    List<Pokemon> _pokemons;

    public void Init()
    {
        _memberSlots = GetComponentsInChildren<PartyMemberUI>();
    }

    public void SetPartyData(List<Pokemon> pokemons)
    {
        _pokemons = pokemons;
        for (int i = 0; i < _memberSlots.Length; i++)
        {
            if (i < pokemons.Count)
            {
                _memberSlots[i].gameObject.SetActive(true);
                _memberSlots[i].SetData(pokemons[i]);
                continue;
            }

            _memberSlots[i].gameObject.SetActive(false);
        }

        UpdateMemberSelection(0);

        messageText.text = "Choose a Pokemon";
    }

    public void UpdateMemberSelection(int selectedMember)
    {
        for (int i = 0; i < _pokemons.Count; i++)
        {
            if (i == selectedMember)
            {
                _memberSlots[i].HightlightPokemon(true);
                continue;
            }
            _memberSlots[i].HightlightPokemon(false);
        }
    }

    public void SetMessageText(string message)
    {
        messageText.text = message;
    }
}
