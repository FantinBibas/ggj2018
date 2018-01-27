using System.Collections;
using UnityEngine;

public class PlayerController : ALivingEntityController
{
    private IEnumerator AwaitOrder()
    {
        
        while (!HasObjective())
        {
            yield return new WaitForEndOfFrame();
        }        
    }
    
    protected override IEnumerator PreTurn()
    {
        yield return AwaitOrder();
    }

    protected override IEnumerator OnObjectiveReached()
    {
        yield return AwaitOrder();
    }
}