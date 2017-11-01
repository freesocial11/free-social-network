namespace mobSocial.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomFieldsSupportV2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.mobSocial_CustomField", "ApplicationId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.mobSocial_CustomField", "ApplicationId");
        }
    }
}
