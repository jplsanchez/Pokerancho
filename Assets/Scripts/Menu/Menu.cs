using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] PokemonParty _playerParty;
    [SerializeField] PokemonParty _foeParty;


    public PokemonParty PlayerParty => _playerParty;
    public PokemonParty FoeParty => _foeParty;
}
