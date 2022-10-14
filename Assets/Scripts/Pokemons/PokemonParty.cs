using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] List<Pokemon> _pokemons;

    public List<Pokemon> Pokemons => _pokemons;

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

    public Pokemon GetAndRestoreRandomPokemon()
    {
        return _pokemons[Random.Range(0, _pokemons.Count)].RestoreHp().RestorePp();
    }
    public Pokemon GetRandomAlivePokemon()
    {
        return _pokemons.Where(p => p.Hp > 0).OrderBy(p => Guid.NewGuid()).FirstOrDefault();
    }
}
