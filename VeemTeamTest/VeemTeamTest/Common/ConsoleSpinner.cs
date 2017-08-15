using System;
using System.Threading;

namespace VeemTeamTest.Common
{
    public class ConsoleSpinner : IDisposable, IConsoleSpinner
    {
        private const string Sequence = @"/-\|";
        private int _counter;
        private readonly int _delay;
        private bool _active;
        private readonly Thread _thread;

        public ConsoleSpinner(int delay = 10)
        {
            _delay = delay;
            _thread = new Thread(Spin);
        }

        public void Start()
        {
            _active = true;
            if (!_thread.IsAlive) _thread.Start();
        }

        public void Stop()
        {
            _active = false;
            Draw(' ');
        }

        private void Spin()
        {
            while (_active)
            {
                Turn();
                Thread.Sleep(_delay);
            }
        }

        private static void Draw(char c)
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(c);
        }

        private void Turn()
        {
            Draw(Sequence[++_counter % Sequence.Length]);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
