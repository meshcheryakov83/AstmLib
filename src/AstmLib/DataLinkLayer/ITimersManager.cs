namespace AstmLib.DataLinkLayer
{
    public interface ITimersManager
    {
        void CreateTimer(string name);

        bool IsTimerInStartedState(string name);

        void StartTimer(string name, int miliseconds);

        bool CheckTimerTimeout(string name);

        void StopTimer(string name);

        void StopAllTimers();
    }
}