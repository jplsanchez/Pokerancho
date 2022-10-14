using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pokemon
{
    [SerializeField] int level;
    [SerializeField] PokemonBase basePokemon;

    public int Level => level;
    public int Hp { get; set; }
    public List<Move> Moves { get; set; }

    public void Setup()
    {
        foreach (var prop in basePokemon.GetType().GetProperties())
        {
            try
            {
                this.GetType().GetProperty(prop.Name).SetValue(this, prop.GetValue(basePokemon, null), null);
            }
            catch (System.Exception)
            {
                Debug.Log($"Error in prop {prop.Name}");
            }
        }

        Hp = MaxHp;

        Moves = new List<Move>();
        foreach (var move in basePokemon.LearnableMoves)
        {
            if (move.Level <= Level) Moves.Add(new(move.MoveBase));

            if (Moves.Count >= 4) break;
        }
    }

    public DamageDetails TakeDamage(Move move, Pokemon attacker)
    {
        DamageDetails damageDetails = CalculateDamage(move, attacker);

        Hp -= damageDetails.Damage;

        if (Hp <= 0)
        {
            damageDetails.isFainted = true;
            Hp = 0;
        }

        return damageDetails;
    }

    private DamageDetails CalculateDamage(Move move, Pokemon attacker)
    {
        float random = Random.Range(0.85f, 1f);
        float type1 = TypeEffectivenessChart.GetEffectiveness(move.Type, this.Type1);
        float type2 = TypeEffectivenessChart.GetEffectiveness(move.Type, this.Type2);

        float critical = 1f;
        if (Random.value * 100f < 6.25f) critical = 2f;

        float modifiers = random * type1 * type2 * critical;
        float a = (2 * attacker.Level + 10) / 250f;

        float d;
        if (move.IsSpecial) d = a * move.Power * ((float)attacker.SpAttack / SpDefense) + 2;
        else d = a * move.Power * ((float)attacker.Attack / Defense) + 2;

        int damage = Mathf.FloorToInt(d * modifiers);

        return new DamageDetails()
        {
            Damage = damage,
            TypeEffectiveness = type1 * type2,
            isFainted = false,
            Critical = critical,
        };
    }

    public Move GetRandomMove()
    {
        var r = Random.Range(0, Moves.Count);
        return Moves[r];
    }

    private int SetStatByLevel(int stat)
    {
        return Mathf.FloorToInt((stat * Level) / 100f) + 5;
    }

    public Pokemon RestoreHp()
    {
        Hp = MaxHp;
        return this;
    }

    public Pokemon RestorePp()
    {
        foreach (var move in Moves)
        {
            move.CurrentPP = move.PP;
        }
        return this;
    }

    // Info
    protected string name;
    protected string description;

    // Sprites
    protected Sprite frontSprite;
    protected Sprite backSprite;

    // Types
    protected PokemonType type1;
    protected PokemonType type2;

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
        get => SetStatByLevel(basePokemon.MaxHp);
        set => basePokemon.MaxHp = value;
    }
    public int Attack
    {
        get => SetStatByLevel(basePokemon.Attack);
        set => basePokemon.Attack = value;
    }
    public int Defense
    {
        get => SetStatByLevel(basePokemon.Defense);
        set => basePokemon.Defense = value;
    }
    public int SpAttack
    {
        get => SetStatByLevel(basePokemon.SpAttack);
        set => basePokemon.SpAttack = value;
    }
    public int SpDefense
    {
        get => SetStatByLevel(basePokemon.SpDefense);
        set => basePokemon.SpDefense = value;
    }
    public int Speed
    {
        get => SetStatByLevel(basePokemon.Speed);
        set => basePokemon.Speed = value;
    }
}

public class DamageDetails
{
    public int Damage { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }
    public bool isFainted { get; set; }
}
