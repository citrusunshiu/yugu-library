using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarNode
{
    public AStarNode parent;
    public Vector3Int position;

    // "g"
    int distanceFromStart;

    // "h"
    int distanceToEnd;

    // "f"
    int totalDistance;

    public AStarNode(AStarNode parent, Vector3Int position, int distanceFromStart, int distanceToEnd)
    {
        this.parent = parent;
        this.position = position;
        this.distanceFromStart = distanceFromStart;
        this.distanceToEnd = distanceToEnd;
        totalDistance = distanceFromStart + distanceToEnd;
    }

    public int GetDistanceFromStart()
    {
        return distanceFromStart;
    }

    public void SetDistanceFromStart(int distanceFromStart)
    {
        this.distanceFromStart = distanceFromStart;
        totalDistance = distanceFromStart + distanceToEnd;
    }
    
    public int GetDistanceToEnd()
    {
        return distanceToEnd;
    }

    public void SetDistanceToEnd(int distanceToEnd)
    {
        this.distanceToEnd = distanceToEnd;
        totalDistance = distanceFromStart + distanceToEnd;
    }

    public int GetTotalDistance()
    {
        return totalDistance;
    }

}
