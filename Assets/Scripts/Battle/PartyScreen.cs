using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField]Text messageText;

    PartyMemberUI[] _membersSlots;

    public void Init()
    {
        _membersSlots = GetComponentsInChildren<PartyMemberUI>();
    }

    public void SetPartyData(List<Pokemon> pokemons)
    {
        for (int i = 0; i < _membersSlots.Length; i++)
        {
            if (i < pokemons.Count)
            {
                _membersSlots[i].gameObject.SetActive(true);
                _membersSlots[i].SetData(pokemons[i]);
                continue;
            }

            _membersSlots[i].gameObject.SetActive(false);
        }

        messageText.text = "Choose a Pokemon";
    }
}
