using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITurnBasedEntity
{
    bool IsCurrentTurn { get; }
    
    IEnumerator DoTurn();
}
