using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Direction {

    public enum to
    {
        NORTH,
        WEST,
        SOUTH,
        EAST,
        NONE
    }

    public static int DirToInt(to dir)
    {
        switch (dir)
        {
            case to.NORTH:
                return 0;
            case to.WEST:
                return 1;
            case to.SOUTH:
                return 2;
            case to.EAST:
                return 3;
            default:
                return -1;
        }
    }    
    
    public static to IntToDir(int val)
    {
        switch (val)
        {
            case 0:
                return to.NORTH;
            case 1:
                return to.WEST;
            case 2:
                return to.SOUTH;
            case 3:
                return to.EAST;
            default:
                return to.NONE;
        }
    }
    
    public static Vector2Int GoNorth(Vector2Int vec, int val)
    {
        return new Vector2Int(vec.x, vec.y - val);
    }    
    
    public static Vector2Int GoWest(Vector2Int vec, int val)
    {
        return new Vector2Int(vec.x - val, vec.y);
    }    
    
    public static Vector2Int GoSouth(Vector2Int vec, int val)
    {
        return new Vector2Int(vec.x, vec.y + val);
    }    
    
    public static Vector2Int GoEast(Vector2Int vec, int val)
    {
        return new Vector2Int(vec.x + val, vec.y);
    }

    public static Vector2Int GoAuto(Vector2Int vec, int val, to dir)
    {
        switch (dir)
        {
            case to.NORTH:
                return GoNorth(vec, val);
            case to.SOUTH:
                return GoSouth(vec, val);
            case to.EAST:
                return GoEast(vec, val);
            case to.WEST:
                return GoWest(vec, val);
            default:
                return vec;
        }
    }

    public static to GetOpposide(to dir)
    {
        switch (dir)
        {
            case to.NORTH:
                return to.SOUTH;
            case to.SOUTH:
                return to.NORTH;
            case to.EAST:
                return to.WEST;
            case to.WEST:
                return to.EAST;
            default:
                return to.NONE;
        }
    }
}