using UnityEngine;

public class PickaxeTrailController : MonoBehaviour
{
    private TrailRenderer trail;

    private void Awake()
    {
        trail = GetComponentInChildren<TrailRenderer>();

        if (trail == null)
        {
            Debug.LogError("TrailRenderer ¾øÀ½");
            return;
        }

        trail.emitting = false;
    }

    public void TrailOn()
    {
        Debug.Log("Trail On");

        trail.Clear();
        trail.emitting = true;
    }

    public void TrailOff()
    {
        Debug.Log("Trail Off");

        trail.emitting = false;
    }
}
