using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    private Pokemon _pokemon;

    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text typeText;
    [SerializeField] HpBar hpBar;
    [SerializeField] Image image;


    [SerializeField] Color hightlightedColor;

    public void SetData(Pokemon pokemon)
    {
        _pokemon = pokemon;

        nameText.text = pokemon.Name;
        levelText.text = "Lvl " + pokemon.Level.ToString();
        hpBar.SetHP((float)pokemon.Hp / pokemon.MaxHp);

        image.sprite = pokemon.FrontSprite;

        StringBuilder typeString =  new(pokemon.Type1.ToString());
        if (pokemon.Type2 != PokemonType.None) typeString.Append($" / {pokemon.Type2}");
        typeText.text = typeString.ToString();
    }

    public void HightlightPokemon(bool selected)
    {
        if (selected) nameText.color = hightlightedColor;
        else nameText.color = new Color(0.1960784f, 0.1960784f, 0.1960784f);
    }
}
