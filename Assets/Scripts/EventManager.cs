public static class EventManager
{
    public static System.Action Tick;

    public static void TriggerTick()
    {
        Tick?.Invoke();
    }
}
