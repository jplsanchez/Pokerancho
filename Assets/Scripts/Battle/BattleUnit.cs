using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] PokemonBase _basePokemon;
    [SerializeField] int level;
    [SerializeField] bool isPlayerUnit;

    public Pokemon Pokemon { get; set; }

    public void Setup()
    {
        Pokemon = new Pokemon(level, _basePokemon);
        if (isPlayerUnit) GetComponent<Image>().sprite = Pokemon.BackSprite;
        else GetComponent<Image>().sprite = Pokemon.FrontSprite;
    }
}
