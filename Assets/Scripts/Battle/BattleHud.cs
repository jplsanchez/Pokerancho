using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HpBar hpBar;
    public void SetData(Pokemon pokemon)
    {
        nameText.text = pokemon.Name;
        levelText.text = "Lvl " + pokemon.Level.ToString();
        hpBar.SetHP((float)pokemon.Hp/pokemon.MaxHp);
    }
}
