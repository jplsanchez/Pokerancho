using System.Collections.Generic;
using UnityEngine;

public class Pokemon : PokemonBase
{
    public int Level { get; set; }
    public int Hp { get; set; }
    public List<Move> Moves { get; set; }


    public Pokemon(int level, PokemonBase basePokemon) : base()
    {

        foreach (var prop in basePokemon.GetType().GetProperties())
        {
            this.GetType().GetProperty(prop.Name).SetValue(this, prop.GetValue(basePokemon, null), null);
        }

        PokemonSetup(level);

    }
    private void PokemonSetup(int level)
    {
        Level = level;
        Hp = MaxHp;

        Moves = new();
        foreach (var move in LearnableMoves)
        {
            if (move.Level <= Level) Moves.Add(new(move.MoveBase));

            if (Moves.Count >= 4) break;
        }
    }

    public bool TakeDamage(Move move, Pokemon attacker)
    {
        float modifiers = Random.Range(0.85f, 1f);
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Power * ((float)attacker.Attack / Defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        Hp -= damage;
        if (Hp <= 0)
        {
            Hp = 0;
            return true;
        }

        return false;
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

    public new int MaxHp
    {
        get => SetStatByLevel(base.MaxHp);
        set => base.MaxHp = value;
    }
    public new int Attack
    {
        get => SetStatByLevel(base.Attack);
        set => base.Attack = value;
    }
    public new int Defense
    {
        get => SetStatByLevel(base.Defense);
        set => base.Defense = value;
    }
    public new int SpAttack
    {
        get => SetStatByLevel(base.SpAttack);
        set => base.SpAttack = value;
    }
    public new int SpDefense
    {
        get => SetStatByLevel(base.SpDefense);
        set => base.SpDefense = value;
    }
    public new int Speed
    {
        get => SetStatByLevel(base.Speed);
        set => base.Speed = value;
    }
}
