using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AstmLib.DataLinkLayer
{
	public class SingleTimer
	{
		private System.Threading.Timer _timer;
		private bool _timerTimeout = true;
		private bool _started = false;

		public SingleTimer()
		{
			_timer = new System.Threading.Timer(new System.Threading.TimerCallback(delegate { _timerTimeout = true; }), null, Timeout.Infinite, Timeout.Infinite);
		}

		public void StartTimer(int milisecond)
		{
			lock (_timer)
			{
				if (_started)
					return;
				_started = true;
				_timerTimeout = false;
				_timer.Change(milisecond, Timeout.Infinite);
			}
		}

		public bool CheckTimerTimeout()
		{
			return _timerTimeout;
		}

		public void StopTimer()
		{
			lock (_timer)
			{
				_started = false;
				_timer.Change(Timeout.Infinite, Timeout.Infinite);
				_timerTimeout = true;
			}
		}

		public bool Started { get { return _started; } }
	}
}
