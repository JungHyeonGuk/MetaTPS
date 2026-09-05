using UnityEngine;

public class Ladder : MonoBehaviour
{
    BoxCollider climbTrigger;
    BoxCollider topFloor;

    public float TopY => climbTrigger.bounds.max.y;
    public Collider TopFloor => topFloor;

    void Awake()
    {
        climbTrigger = GetComponent<BoxCollider>();
        topFloor = gameObject.AddComponent<BoxCollider>();
        topFloor.size = new Vector3(1.2f, 0.06f, 1.2f);
        topFloor.center = new Vector3(0f, 0.5f, 0f);
    }
}
