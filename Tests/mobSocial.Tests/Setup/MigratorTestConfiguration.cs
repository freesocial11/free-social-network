// #region Author Information
// // MigratorTestConfiguration.cs
// // 
// // (c) 2017 Apexol Technologies. All Rights Reserved.
// // 
// #endregion

using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using mobSocial.Data.Database;

namespace mobSocial.Tests.Setup
{
    public class MigratorTestConfiguration : DbMigrationsConfiguration<DatabaseContext>
    {
        public MigratorTestConfiguration(string connectionString, string providerName)
        {
            AutomaticMigrationsEnabled = true;

            TargetDatabase = new DbConnectionInfo(connectionString, providerName);

        }
    }
}