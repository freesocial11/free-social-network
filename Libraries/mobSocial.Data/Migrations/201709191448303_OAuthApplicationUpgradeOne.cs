namespace mobSocial.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OAuthApplicationUpgradeOne : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.mobSocial_OAuthApplication", "UserId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.mobSocial_OAuthApplication", "UserId");
        }
    }
}
