using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    private Pokemon _pokemon;

    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HpBar hpBar;
    public void SetData(Pokemon pokemon)
    {
        _pokemon = pokemon;

        nameText.text = pokemon.Name;
        levelText.text = "Lvl " + pokemon.Level.ToString();
        hpBar.SetHP((float)pokemon.Hp / pokemon.MaxHp);
    }

    public IEnumerator UpdateHp()
    {
        yield return hpBar.SetHpSmooth((float)_pokemon.Hp / _pokemon.MaxHp);
    }
}
