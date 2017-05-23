// #region Author Information
// // PostInstallationTasks.cs
// // 
// // (c) Apexol Technologies. All Rights Reserved.
// // 
// #endregion

using System;
using System.Collections.Generic;
using System.Linq;
using DryIoc;
using mobSocial.Core.Infrastructure.AppEngine;

namespace mobSocial.Data.Database.Initializer
{
    public static class PostInstallationTasks
    {
        private static IList<Action> _tasks;

        static PostInstallationTasks()
        {
            _tasks = new List<Action>();
        }
        public static void RunPostInstallation(Action task)
        {
            _tasks.Add(task);
        }

        public static bool HasPostInstallationTasks()
        {
            return _tasks.Any();
        }

        public static void Execute()
        {
            foreach (var task in _tasks)
                task();
        }
    }
}