using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class Figure
{
    public string _id {  get; private set; }
    public PlayerType type { get; private set; }

    public Figure() { }
    public Figure(string id, PlayerType type)
    {
        _id = id;
        this.type = type;
        // add all of the stats
    }
}
