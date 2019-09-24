using System.Collections.Generic;

namespace AstmLib.DataLinkLayer
{
    public class TimersManager : ITimersManager
    {
        private readonly Dictionary<string, SingleTimer> _timers = new Dictionary<string, SingleTimer>();

        public void CreateTimer(string name)
        {
            _timers.Add(name, new SingleTimer());
        }

        public bool IsTimerInStartedState(string name)
        {
            if (!_timers.ContainsKey(name))
            {
                return false;
            }

            return _timers[name].Started;
        }

        public void StartTimer(string name, int miliseconds)
        {
            lock (_timers)
            {
                if (!_timers.ContainsKey(name))
                {
                    _timers.Add(name, new SingleTimer());
                }

                _timers[name].StopTimer();
                _timers[name].StartTimer(miliseconds);
            }
        }

        public bool CheckTimerTimeout(string name)
        {
            return _timers[name].CheckTimerTimeout();
        }

        public void StopTimer(string name)
        {
            _timers[name].StopTimer();
        }

        public void StopAllTimers()
        {
            foreach (var singleTimer in _timers.Values)
            {
                singleTimer.StopTimer();
            }
        }
    }
}