namespace mobSocial.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PerApplicationEntity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.mobSocial_TimelinePost", "ApplicationId", c => c.Int(nullable: false));
            AddColumn("dbo.mobSocial_Comment", "ApplicationId", c => c.Int(nullable: false));
            AddColumn("dbo.mobSocial_UserFollow", "ApplicationId", c => c.Int(nullable: false));
            AddColumn("dbo.mobSocial_Friend", "ApplicationId", c => c.Int(nullable: false));
            AddColumn("dbo.mobSocial_UserLike", "ApplicationId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.mobSocial_UserLike", "ApplicationId");
            DropColumn("dbo.mobSocial_Friend", "ApplicationId");
            DropColumn("dbo.mobSocial_UserFollow", "ApplicationId");
            DropColumn("dbo.mobSocial_Comment", "ApplicationId");
            DropColumn("dbo.mobSocial_TimelinePost", "ApplicationId");
        }
    }
}
