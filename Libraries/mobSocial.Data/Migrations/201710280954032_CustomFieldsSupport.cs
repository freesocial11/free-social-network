namespace mobSocial.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomFieldsSupport : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.mobSocial_CustomField",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EntityName = c.String(),
                        Label = c.String(),
                        Description = c.String(),
                        FieldType = c.Int(nullable: false),
                        Visible = c.Boolean(nullable: false),
                        Required = c.Boolean(nullable: false),
                        DefaultValue = c.String(),
                        ParentFieldId = c.Int(),
                        ParentFieldValue = c.String(),
                        FieldGeneratorMarkup = c.String(),
                        DisplayOrder = c.Int(nullable: false),
                        MinimumValue = c.String(),
                        MaximumValue = c.String(),
                        AvailableValues = c.String(),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.mobSocial_CustomField", t => t.ParentFieldId)
                .Index(t => t.ParentFieldId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.mobSocial_CustomField", "ParentFieldId", "dbo.mobSocial_CustomField");
            DropIndex("dbo.mobSocial_CustomField", new[] { "ParentFieldId" });
            DropTable("dbo.mobSocial_CustomField");
        }
    }
}
