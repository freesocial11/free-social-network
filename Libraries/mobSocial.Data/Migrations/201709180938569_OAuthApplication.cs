namespace mobSocial.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OAuthApplication : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.mobSocial_AppToken",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Guid = c.String(nullable: false),
                        Subject = c.String(nullable: false),
                        ClientId = c.String(nullable: false),
                        IssuedUtc = c.DateTime(nullable: false),
                        ExpiresUtc = c.DateTime(nullable: false),
                        ProtectedTicket = c.String(nullable: false),
                        TokenType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.mobSocial_OAuthApplication",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Guid = c.String(),
                        Secret = c.String(),
                        Name = c.String(),
                        Description = c.String(),
                        ApplicationUrl = c.String(),
                        PrivacyPolicyUrl = c.String(),
                        TermsUrl = c.String(),
                        RedirectUrl = c.String(),
                        ApplicationType = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        RefreshTokenLifeTime = c.Int(nullable: false),
                        AllowedOrigin = c.String(),
                        RequestLimitPerHour = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.mobSocial_OAuthApplication");
            DropTable("dbo.mobSocial_AppToken");
        }
    }
}
