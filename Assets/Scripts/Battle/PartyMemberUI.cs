using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    private Pokemon _pokemon;

    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HpBar hpBar;

    [SerializeField] Color hightlightedColor;

    public void SetData(Pokemon pokemon)
    {
        _pokemon = pokemon;

        nameText.text = pokemon.Name;
        levelText.text = "Lvl " + pokemon.Level.ToString();
        hpBar.SetHP((float)pokemon.Hp / pokemon.MaxHp);
    }

    public void HightlightPokemon(bool selected)
    {
        if (selected) nameText.color = hightlightedColor;
        else nameText.color = Color.black;
    }
}
