using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
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
}
