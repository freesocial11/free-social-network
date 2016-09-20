using System;

namespace mobSocial.Core.Tasks
{
    public interface ITask : IDisposable
    {
        void Run();

        string SystemName { get; }
    }
}