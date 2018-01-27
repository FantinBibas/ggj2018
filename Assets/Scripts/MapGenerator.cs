using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : AMapGenerator
{
    public Grid Grid;
    
    public override void GenerateMap(Grid grid)
    {
        Grid = grid;
    }
    
}
