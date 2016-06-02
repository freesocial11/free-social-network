using mobSocial.Core.Startup;

namespace mobSocial.Data.Tasks
{
    public class DatabaseSetupTask : IStartupTask
    {

        public void Run()
        {
            //we will run the migrator only if database exists
          /*  var dbExists = mobSocialEngine.ActiveEngine.Resolve<IDatabaseContext>().DatabaseExists();
            if (!dbExists)
                return;*/
            //run the migrator. this will update any pending tasks or updates to database
           // var migrator = new DbMigrator(new Configuration());
           // migrator.Update();
        }

        public int Priority
        {
            get { return -int.MaxValue; } //should be the first task ever
        }
    }
}