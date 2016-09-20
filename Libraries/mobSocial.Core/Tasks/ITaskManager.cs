using System;

namespace mobSocial.Core.Tasks
{
    public interface ITaskManager
    {
        void Start(Type[] availableTaskTypes);
    }
}