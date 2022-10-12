using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create new pokemon")]
public class PokemonBase : ScriptableObject
{
    // Info
    [SerializeField] protected new string name;

    [TextArea]
    [SerializeField] protected string description;

    // Sprites
    [SerializeField] protected Sprite frontSprite;
    [SerializeField] protected Sprite backSprite;

    // Types
    [SerializeField] protected PokemonType type1;
    [SerializeField] protected PokemonType type2;

    // Stats
    [SerializeField] protected int maxHp;
    [SerializeField] protected int attack;
    [SerializeField] protected int defense;
    [SerializeField] protected int spAttack;
    [SerializeField] protected int spDefense;
    [SerializeField] protected int speed;

    // Moves
    [SerializeField] List<LearnableMove> learnableMoves;


    // Properties
    public string Name
    {
        get => name;
        set => name = value;
    }
    public string Description
    {
        get => description;
        set => description = value;
    }
    public Sprite FrontSprite
    {
        get => frontSprite;
        set => frontSprite = value;
    }
    public Sprite BackSprite
    {
        get => backSprite;
        set => backSprite = value;
    }
    public PokemonType Type1
    {
        get => type1;
        set => type1 = value;
    }
    public PokemonType Type2
    {
        get => type2;
        set => type2 = value;
    }
    public int MaxHp
    {
        get => maxHp;
        set => maxHp = value;
    }
    public int Attack
    {
        get => attack;
        set => attack = value;
    }
    public int Defense
    {
        get => defense;
        set => defense = value;
    }
    public int SpAttack
    {
        get => spAttack;
        set => spAttack = value;
    }
    public int SpDefense
    {
        get => spDefense;
        set => spDefense = value;
    }
    public int Speed
    {
        get => speed;
        set => speed = value;
    }
    public List<LearnableMove> LearnableMoves
    {
        get => learnableMoves;
        set => learnableMoves = value;
    }
}

[System.Serializable]
public class LearnableMove
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public MoveBase MoveBase => moveBase;
    public int Level => level;

}

public enum StatType
{
    None,
    MaxHp,
    Attack,
    Defense,
    SpAttack,
    SpDefense,
    Speed
}

public enum PokemonType
{
    None,
    Normal,
    Fire,
    Water,
    Grass,
    Flying,
    Fighting,
    Poison,
    Electric,
    Ground,
    Rock,
    Psychic,
    Ice,
    Bug,
    Ghost,
    Steel,
    Dragon,
    Dark,
    Fairy
}
