
using UnityEngine;
public class utils
{
    public static float FlatSqrDistance(Vector2 firstVec, Vector2 secondVec)
    {
        return (firstVec - secondVec).sqrMagnitude;
    }
    public static float FlatSqrDistance(Vector3 firstVec, Vector2 secondVec)
    {
        return (new Vector2(firstVec.x, firstVec.y) - secondVec).sqrMagnitude;
    }
    public static float FlatSqrDistance(Vector2 firstVec, Vector3 secondVec)
    {
        return (firstVec - new Vector2(secondVec.x, secondVec.y)).sqrMagnitude;
    }
    public static float FlatSqrDistance(Vector3 firstVec, Vector3 secondVec)
    {
        return new Vector2(firstVec.x - secondVec.x, firstVec.y - secondVec.y).sqrMagnitude;
    }
}