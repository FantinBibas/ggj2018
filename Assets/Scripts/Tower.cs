using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public BasicActivable[] Nodes;

    public uint getNodesNumber()
    {
        return (uint) Nodes.Length;
    }

    public uint getActivatedNodes()
    {
        uint i = 0;
        foreach (BasicActivable node in Nodes)
        {
            if (node.status)
                i++;
        }

        return i;
    }

    public uint getNotActivatedNodes()
    {
        return getNodesNumber() - getActivatedNodes();
    }

    public bool isActivated()
    {
        return getNotActivatedNodes() == 0;
    }
}
