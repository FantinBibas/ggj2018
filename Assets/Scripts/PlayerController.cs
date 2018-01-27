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
        if (RemainingMoves > 0)
            yield return AwaitOrder();
    }

    protected override IEnumerator PostTurn()
    {
        CancelObjective();
        yield return base.PostTurn();
    }
}