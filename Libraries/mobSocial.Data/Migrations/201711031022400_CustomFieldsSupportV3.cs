namespace mobSocial.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomFieldsSupportV3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.mobSocial_CustomField", "SystemName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.mobSocial_CustomField", "SystemName");
        }
    }
}
