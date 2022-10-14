using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Move", menuName ="Pokemon/Create new move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] protected new string name;

    [TextArea]
    [SerializeField] protected string description;

    [SerializeField] protected PokemonType type;
    [SerializeField] protected int power;
    [SerializeField] protected int accuracy;
    [SerializeField] protected int priotity;
    [SerializeField] protected int pp;
    [SerializeField] protected StatType statType;
    [SerializeField] protected int statStage;
    [SerializeField] protected bool isSpecial;

    // Properties
    public string Name => name;
    public string Description => description;
    public PokemonType Type => type;
    public int Power => power;
    public int Accuracy => accuracy;
    public int Priotity => priotity;
    public int PP => pp;
    public StatType StatType => statType;
    public int StatStage => statStage;
    public bool IsSpecial => isSpecial;
}
