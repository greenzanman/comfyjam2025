
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
        public static Vector2 MoveTowardsOffset(Vector2 start, Vector2 goal, float buffer, float rate, float delta, bool orCloser = true)
    {
        float dist = (goal - start).magnitude;
        if (orCloser)
        {
            if (dist - buffer < 0)
            {
                return start;
            }
            if (dist - buffer < rate * delta)
            {
                // Return the spot a distance 'buffer' away
                return goal + (start - goal) / dist * buffer;
            }
            return start + (goal - start) / dist * rate * delta;
        }
        else
        {
            if (Mathf.Abs(dist - buffer) < rate * delta)
            {
                return goal - (goal - start) / dist * buffer;
            }
            return start + (goal - start) / dist * rate * delta * Mathf.Sign(dist);
        }
    }
}