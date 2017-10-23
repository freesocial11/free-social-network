namespace mobSocial.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApplicationUsageTracking : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.mobSocial_ApplicationUsage",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ApplicationId = c.Int(nullable: false),
                        UsageCount = c.Int(nullable: false),
                        LastRequested = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.mobSocial_ApplicationUsage");
        }
    }
}
