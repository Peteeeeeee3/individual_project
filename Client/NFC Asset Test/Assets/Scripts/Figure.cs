using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class Figure
{
    public string id {  get; private set; }
    public PlayerType type { get; private set; }
    public int level { get; private set; }
    public int availableUpgrades { get; private set; }
    public int exp { get; private set; }
    public float moveSpeed { get; set; }
    public float damage { get; set; }
    public float attackRate { get; set; }
    public float attackRange { get; set; }
    public string name { get; private set; }

    public Figure() { }
    public Figure(string id, PlayerType type, int level, int availableUpgrades, int exp, float moveSpeed, float damage, float attackRate, float attackRange, string name)
    {
        this.id = id;
        this.type = type;
        this.level = level;
        this.availableUpgrades = availableUpgrades;
        this.exp = exp;
        this.moveSpeed = moveSpeed;
        this.damage = damage;
        this.attackRate = attackRate;
        this.attackRange = attackRange;
        this.name = name;
    }
}
