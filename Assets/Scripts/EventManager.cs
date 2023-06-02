public static class EventManager
{
    public static System.Action Tick;

    public static void TriggerTick()
    {
        Tick?.Invoke();
    }

    public static System.Action<TockInfo> Tock;

    public static void TriggerTock(TockInfo info)
    {
        Tock?.Invoke(info);
    }
}
