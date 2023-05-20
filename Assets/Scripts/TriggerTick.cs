using UnityEngine;

public class TriggerTick : MonoBehaviour
{
    public void Trigger()
    {
        EventManager.TriggerTick();
    }
}
