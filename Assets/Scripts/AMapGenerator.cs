using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AMapGenerator : MonoBehaviour
{
    public abstract void GenerateMap(Grid grid);
}
