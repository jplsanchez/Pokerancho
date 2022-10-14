using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] List<Pokemon> _pokemons;

    private void Start()
    {
        foreach (var pokemon in _pokemons)
        {
            pokemon.Setup();
        }
    }

    public Pokemon GetFirstHealthyPokemon()
    {
        return _pokemons.Where(p => p.Hp > 0).FirstOrDefault();
    }

    public Pokemon GetRandomPokemon()
    {
        return _pokemons[Random.Range(0, _pokemons.Count)];
    }
}
