﻿using System.Collections;
using System.Linq;
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
        //  if (RemainingMoves > 0)
        //      yield return AwaitOrder();
        yield break;
    }

    protected override IEnumerator PostTurn()
    {
        CancelObjective();
        yield return base.PostTurn();
    }

protected override IEnumerator OnMove()
    {
        foreach (GuardController g in GameManager.Instance.Guards)
            g.CheckForPlayer();
        if (!GameManager.Instance.Map.IsStation(Position)) yield break;
        yield return GameManager.Instance.ShowMinigame();
        GameManager.Instance.Map.RemoveStationAt(Position);
        if (GameManager.Instance.Map.Stations.Count == 0)
            GameManager.Instance.Win();
    }
}