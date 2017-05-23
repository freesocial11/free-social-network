using System.Data.Entity;
using DryIoc;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Data.Database;
using mobSocial.Data.Database.Initializer;
using mobSocial.Data.Extensions;
using mobSocial.Data.Integration;

namespace mobSocial.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.mobSocial_CustomerVideo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CustomerVideoAlbumId = c.Int(nullable: false),
                        VideoUrl = c.String(),
                        Caption = c.String(),
                        DisplayOrder = c.Int(nullable: false),
                        LikeCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.mobSocial_VideoGenre",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GenreName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.mobSocial_WatchedVideo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VideoId = c.Int(nullable: false),
                        CustomerId = c.Int(nullable: false),
                        VideoType = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.mobSocial_RoleCapability",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoleId = c.Int(nullable: false),
                        CapabilityId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.mobSocial_Capability", t => t.CapabilityId, cascadeDelete: true)
                .ForeignKey("dbo.mobSocial_Role", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.RoleId)
                .Index(t => t.CapabilityId);
            
            CreateTable(
                "dbo.mobSocial_Capability",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CapabilityName = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.mobSocial_Role",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoleName = c.String(),
                        IsSystemRole = c.Boolean(nullable: false),
                        SystemName = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            //role map
            var roleIntegrationMap = IntegrationManager.RoleIntegrationMap ?? new RoleIntegrationMap();
            var roleView = roleIntegrationMap.GetView();
            Sql(roleView);

            CreateTable(
                "dbo.mobSocial_User",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Name = c.String(),
                        Email = c.String(nullable: false),
                        UserName = c.String(),
                        Guid = c.Guid(nullable: false),
                        Password = c.String(nullable: false),
                        PasswordSalt = c.String(),
                        PasswordFormat = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                        LastLoginDate = c.DateTime(),
                        IsSystemAccount = c.Boolean(nullable: false),
                        Remarks = c.String(),
                        LastLoginIpAddress = c.String(),
                        ReferrerId = c.Int(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            //create view map for user. This can be overridden by other integrating applications 
            var userIntegrationMap = (IntegrationManager.UserIntegrationMap ?? new UserIntegrationMap());
            var userViewMap = userIntegrationMap.GetView();
            Sql(userViewMap);

            CreateTable(
                "dbo.mobSocial_Education",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        FromDate = c.DateTime(),
                        ToDate = c.DateTime(),
                        UserId = c.Int(nullable: false),
                        SchoolId = c.Int(nullable: false),
                        EducationType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.mobSocial_School", t => t.SchoolId, cascadeDelete: true)
                .ForeignKey("dbo.mobSocial_User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.SchoolId);
            
            CreateTable(
                "dbo.mobSocial_School",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        City = c.String(),
                        LogoId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.mobSocial_UserRole",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.mobSocial_Role", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.mobSocial_User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            var userRoleIntegrationMap = (IntegrationManager.UserRoleIntegrationMap ?? new UserRoleIntegrationMap());
            var userRoleViewMap = userRoleIntegrationMap.GetView();
            Sql(userRoleViewMap);

            CreateTable(
                "dbo.mobSocial_TimelinePost",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OwnerId = c.Int(nullable: false),
                        OwnerEntityType = c.String(),
                        PostTypeName = c.String(),
                        IsSponsored = c.Boolean(nullable: false),
                        Message = c.String(),
                        AdditionalAttributeValue = c.String(),
                        LinkedToEntityId = c.Int(nullable: false),
                        LinkedToEntityName = c.String(),
                        IsHidden = c.Boolean(nullable: false),
                        PublishDate = c.DateTime(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                        InlineTags = c.String(),
                        ExternalTags = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.mobSocial_TeamPage",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        UpdatedBy = c.Int(nullable: false),
                        Description = c.String(),
                        TeamPictureId = c.Int(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.mobSocial_GroupPage",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        PayPalDonateUrl = c.String(),
                        TeamPageId = c.Int(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        IsDefault = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.mobSocial_TeamPage", t => t.TeamPageId, cascadeDelete: true)
                .Index(t => t.TeamPageId);
            
            CreateTable(
                "dbo.mobSocial_GroupPageMember",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GroupPageId = c.Int(nullable: false),
                        CustomerId = c.Int(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.mobSocial_GroupPage", t => t.GroupPageId, cascadeDelete: true)
                .Index(t => t.GroupPageId);
            
            CreateTable(
                "dbo.mobSocial_Sponsor",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        BattleId = c.Int(nullable: false),
                        BattleType = c.Int(nullable: false),
                        SponsorshipAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SponsorshipStatus = c.Int(nullable: false),
                        SponsorshipType = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.mobSocial_SponsorData",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BattleId = c.Int(nullable: false),
                        BattleType = c.Int(nullable: false),
                        SponsorCustomerId = c.Int(nullable: false),
                        PictureId = c.Int(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        TargetUrl = c.String(),
                        DisplayName = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.mobSocial_SponsorPass",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        SponsorPassOrderId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        BattleId = c.Int(nullable: false),
                        BattleType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.mobSocial_SharedSong",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        SenderId = c.Int(nullable: false),
                        SongId = c.Int(nullable: false),
                        Message = c.String(),
                        SharedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.mobSocial_Song", t => t.SongId, cascadeDelete: true)
                .Index(t => t.SongId);
            
            CreateTable(
                "dbo.mobSocial_Song",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PageOwnerId = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        RemoteEntityId = c.String(),
                        RemoteSourceName = c.String(),
                        PreviewUrl = c.String(),
                        TrackId = c.String(),
                        RemoteArtistId = c.String(),
                        ArtistPageId = c.Int(),
                        AssociatedProductId = c.Int(nullable: false),
                        Published = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.mobSocial_ArtistPage", t => t.ArtistPageId)
                .Index(t => t.ArtistPageId);
            
            CreateTable(
                "dbo.mobSocial_ArtistPage",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PageOwnerId = c.Int(nullable: false),
                        Name = c.String(),
                        RemoteEntityId = c.String(),
                        RemoteSourceName = c.String(),
                        Biography = c.String(),
                        DateOfBirth = c.DateTime(nullable: false),
                        Gender = c.String(),
                        HomeTown = c.String(),
                        ShortDescription = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.mobSocial_ArtistPageManager",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        ArtistPageId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.mobSocial_ArtistPage", t => t.ArtistPageId, cascadeDelete: true)
                .Index(t => t.ArtistPageId);
            
            CreateTable(
                "dbo.mobSocial_Comment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        EntityId = c.Int(nullable: false),
                        EntityName = c.String(),
                        CommentText = c.String(),
                        AdditionalData = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        InlineTags = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.mobSocial_UserFollow",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        FollowingEntityId = c.Int(nullable: false),
                        FollowingEntityName = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.mobSocial_Friend",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FromCustomerId = c.Int(nullable: false),
                        ToCustomerId = c.Int(nullable: false),
                        Confirmed = c.Boolean(nullable: false),
                        DateRequested = c.DateTime(nullable: false),
                        DateConfirmed = c.DateTime(),
                        NotificationCount = c.Int(nullable: false),
                        LastNotificationDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.mobSocial_UserLike",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        EntityId = c.Int(nullable: false),
                        EntityName = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.mobSocial_Permalink",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Slug = c.String(),
                        Active = c.Boolean(nullable: false),
                        EntityId = c.Int(nullable: false),
                        EntityName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.mobSocial_EntityMedia",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EntityId = c.Int(nullable: false),
                        EntityName = c.String(),
                        MediaId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.mobSocial_Media",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        SystemName = c.String(),
                        Description = c.String(),
                        AlternativeText = c.String(),
                        LocalPath = c.String(),
                        ThumbnailPath = c.String(),
                        MimeType = c.String(),
                        Binary = c.Binary(),
                        MediaType = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.mobSocial_UserPaymentMethod",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        PaymentMethodType = c.Int(nullable: false),
                        CardNumber = c.String(),
                        CardNumberMasked = c.String(),
                        NameOnCard = c.String(),
                        ExpireMonth = c.String(),
                        ExpireYear = c.String(),
                        CardIssuerType = c.String(),
                        IsVerified = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.mobSocial_PaymentTransaction",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TransactionGuid = c.Guid(nullable: false),
                        PaymentStatus = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        UserId = c.Int(nullable: false),
                        BillingAddressId = c.Int(nullable: false),
                        TransactionAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UserIpAddress = c.String(),
                        UserPaymentMethodId = c.Int(nullable: false),
                        PaymentProcessorSystemName = c.String(),
                        IsLocalTransaction = c.Boolean(nullable: false),
                        ThirdPartyAppId = c.Int(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.mobSocial_EventPage",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        LocationName = c.String(),
                        LocationAddress1 = c.String(),
                        LocationAddress2 = c.String(),
                        LocationCity = c.String(),
                        LocationState = c.String(),
                        LocationZipPostalCode = c.String(),
                        LocationCountry = c.String(),
                        LocationPhone = c.String(),
                        LocationWebsite = c.String(),
                        LocationEmail = c.String(),
                        Description = c.String(),
                        MetaKeywords = c.String(),
                        MetaDescription = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.mobSocial_EventPageHotel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EventPageId = c.Int(nullable: false),
                        Title = c.String(),
                        Name = c.String(),
                        Address1 = c.String(),
                        Address2 = c.String(),
                        City = c.String(),
                        State = c.String(),
                        ZipPostalCode = c.String(),
                        Country = c.String(),
                        PhoneNumber = c.String(),
                        AdditionalInformation = c.String(),
                        DisplayOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.mobSocial_EventPage", t => t.EventPageId, cascadeDelete: true)
                .Index(t => t.EventPageId);
            
            CreateTable(
                "dbo.mobSocial_EventPageAttendance",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EventPageId = c.Int(nullable: false),
                        CustomerId = c.Int(nullable: false),
                        AttendanceStatusId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.mobSocial_EventPage", t => t.EventPageId, cascadeDelete: true)
                .Index(t => t.EventPageId);
            
            CreateTable(
                "dbo.mobSocial_EntityProperty",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EntityId = c.Int(nullable: false),
                        EntityName = c.String(),
                        PropertyName = c.String(),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.mobSocial_Skill",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DisplayOrder = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        FeaturedImageId = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.mobSocial_UserSkill",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SkillId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        ExternalUrl = c.String(),
                        DisplayOrder = c.Int(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.mobSocial_Skill", t => t.SkillId, cascadeDelete: true)
                .ForeignKey("dbo.mobSocial_User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.SkillId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.mobSocial_Setting",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GroupName = c.String(),
                        Key = c.String(),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.mobSocial_ScheduledTask",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Seconds = c.Int(nullable: false),
                        SystemName = c.String(),
                        Enabled = c.Boolean(nullable: false),
                        IsRunning = c.Boolean(nullable: false),
                        LastStartDateTime = c.DateTime(),
                        LastEndDateTime = c.DateTime(),
                        LastSuccessDateTime = c.DateTime(),
                        StopOnError = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.mobSocial_Notification",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        IsRead = c.Boolean(nullable: false),
                        PublishDateTime = c.DateTime(nullable: false),
                        ReadDateTime = c.DateTime(),
                        EntityId = c.Int(nullable: false),
                        EntityName = c.String(),
                        NotificationEventId = c.Int(),
                        InitiatorId = c.Int(nullable: false),
                        InitiatorName = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.mobSocial_NotificationEvent", t => t.NotificationEventId)
                .ForeignKey("dbo.mobSocial_User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.NotificationEventId);
            
            CreateTable(
                "dbo.mobSocial_NotificationEvent",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EventName = c.String(),
                        Enabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.mobSocial_EmailAccount",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(),
                        FromName = c.String(),
                        Host = c.String(),
                        Port = c.Int(nullable: false),
                        UserName = c.String(),
                        Password = c.String(),
                        UseSsl = c.Boolean(nullable: false),
                        UseDefaultCredentials = c.Boolean(nullable: false),
                        IsDefault = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.mobSocial_EmailMessage",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TosSerialized = c.String(),
                        CcsSerialized = c.String(),
                        BccsSerialized = c.String(),
                        ReplyTosSerialized = c.String(),
                        Subject = c.String(),
                        EmailBody = c.String(),
                        IsEmailBodyHtml = c.Boolean(nullable: false),
                        AttachmentsSerialized = c.String(),
                        EmailAccountId = c.Int(nullable: false),
                        SendingDate = c.DateTime(nullable: false),
                        IsSent = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.mobSocial_EmailAccount", t => t.EmailAccountId, cascadeDelete: true)
                .Index(t => t.EmailAccountId);
            
            CreateTable(
                "dbo.mobSocial_EmailTemplate",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TemplateName = c.String(),
                        TemplateSystemName = c.String(),
                        Template = c.String(),
                        IsMaster = c.Boolean(nullable: false),
                        ParentEmailTemplateId = c.Int(),
                        EmailAccountId = c.Int(),
                        Subject = c.String(),
                        AdministrationEmail = c.String(),
                        IsSystem = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.mobSocial_EmailAccount", t => t.EmailAccountId)
                .ForeignKey("dbo.mobSocial_EmailTemplate", t => t.ParentEmailTemplateId)
                .Index(t => t.ParentEmailTemplateId)
                .Index(t => t.EmailAccountId);
            
            CreateTable(
                "dbo.mobSocial_Currency",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CurrencyName = c.String(),
                        DisplayFormat = c.String(),
                        DisplayLocale = c.String(),
                        CurrencyCode = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.mobSocial_Credit",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        CreditType = c.Int(nullable: false),
                        CreditTransactionType = c.Int(nullable: false),
                        CreditContextKey = c.String(),
                        CreditCount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreditExchangeRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        ExpiresOnUtc = c.DateTime(nullable: false),
                        IsExpired = c.Boolean(nullable: false),
                        Remarks = c.String(),
                        PaymentTransactionId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.mobSocial_PaymentTransaction", t => t.PaymentTransactionId)
                .Index(t => t.PaymentTransactionId);
            
            CreateTable(
                "dbo.mobSocial_Conversation",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        ReceiverId = c.Int(nullable: false),
                        ReceiverType = c.String(),
                        ReceiverDeleted = c.Boolean(nullable: false),
                        SenderDeleted = c.Boolean(nullable: false),
                        LastUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.mobSocial_User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.mobSocial_ConversationReply",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ConversationId = c.Int(nullable: false),
                        ReplyText = c.String(),
                        UserId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        IpAddress = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.mobSocial_Conversation", t => t.ConversationId, cascadeDelete: true)
                .ForeignKey("dbo.mobSocial_User", t => t.UserId)
                .Index(t => t.ConversationId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.mobSocial_ConversationReplyStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ReplyId = c.Int(nullable: false),
                        ReplyStatus = c.Int(nullable: false),
                        LastUpdated = c.DateTime(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.mobSocial_ConversationReply", t => t.ReplyId, cascadeDelete: true)
                .Index(t => t.ReplyId);
            
            CreateTable(
                "dbo.mobSocial_BusinessPage",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CountryId = c.Int(nullable: false),
                        Address1 = c.String(),
                        Address2 = c.String(),
                        City = c.String(),
                        StateId = c.Int(nullable: false),
                        ZipPostalCode = c.String(),
                        Phone = c.String(),
                        Website = c.String(),
                        Email = c.String(),
                        Description = c.String(),
                        MetaKeywords = c.String(),
                        MetaDescription = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.mobSocial_BusinessPageCoupon",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BusinessPageId = c.Int(nullable: false),
                        Title = c.String(),
                        Name = c.String(),
                        BriefDescription = c.String(),
                        Disclaimer = c.String(),
                        UsageCount = c.Int(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.mobSocial_BusinessPage", t => t.BusinessPageId, cascadeDelete: true)
                .Index(t => t.BusinessPageId);
            
            CreateTable(
                "dbo.mobSocial_VideoBattle",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        ChallengerId = c.Int(nullable: false),
                        VotingStartDate = c.DateTime(nullable: false),
                        VotingEndDate = c.DateTime(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                        VideoBattleType = c.Int(nullable: false),
                        VideoBattleStatus = c.Int(nullable: false),
                        VideoBattleVoteType = c.Int(nullable: false),
                        ParticipationType = c.Int(nullable: false),
                        MaximumParticipantCount = c.Int(nullable: false),
                        IsVotingPayable = c.Boolean(nullable: false),
                        MinimumVotingCharge = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CanVoterIncreaseVotingCharge = c.Boolean(nullable: false),
                        ParticipantPercentagePerVote = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsSponsorshipSupported = c.Boolean(nullable: false),
                        MinimumSponsorshipAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SponsoredCashDistributionType = c.Int(nullable: false),
                        AutomaticallyPostEventsToTimeline = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.mobSocial_VideoBattlePrize",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VideoBattleId = c.Int(nullable: false),
                        WinnerPosition = c.Int(nullable: false),
                        PrizeType = c.Int(nullable: false),
                        Description = c.String(),
                        PrizePercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PrizeAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PrizeProductId = c.Int(nullable: false),
                        PrizeOther = c.String(),
                        WinnerId = c.Int(nullable: false),
                        IsSponsored = c.Boolean(nullable: false),
                        SponsorCustomerId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.mobSocial_VideoBattle", t => t.VideoBattleId, cascadeDelete: true)
                .Index(t => t.VideoBattleId);
            
            CreateTable(
                "dbo.mobSocial_VideoBattleGenre",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VideoBattleId = c.Int(nullable: false),
                        VideoGenreId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.mobSocial_VideoBattle", t => t.VideoBattleId, cascadeDelete: true)
                .ForeignKey("dbo.mobSocial_VideoGenre", t => t.VideoGenreId, cascadeDelete: true)
                .Index(t => t.VideoBattleId)
                .Index(t => t.VideoGenreId);
            
            CreateTable(
                "dbo.mobSocial_VideoBattleParticipant",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VideoBattleId = c.Int(nullable: false),
                        ParticipantId = c.Int(nullable: false),
                        ParticipantStatus = c.Int(nullable: false),
                        LastUpdated = c.DateTime(nullable: false),
                        Remarks = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.mobSocial_VideoBattle", t => t.VideoBattleId, cascadeDelete: true)
                .Index(t => t.VideoBattleId);
            
            CreateTable(
                "dbo.mobSocial_VideoBattleVideo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VideoPath = c.String(),
                        MimeType = c.String(),
                        ParticipantId = c.Int(nullable: false),
                        VideoBattleId = c.Int(nullable: false),
                        VideoStatus = c.Int(nullable: false),
                        ThumbnailPath = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.mobSocial_VideoBattle", t => t.VideoBattleId, cascadeDelete: true)
                .Index(t => t.VideoBattleId);
            
            CreateTable(
                "dbo.mobSocial_VideoBattleView",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        VideoBattleVideoId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.mobSocial_VideoBattleVote",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        VideoBattleId = c.Int(nullable: false),
                        ParticipantId = c.Int(nullable: false),
                        VoteValue = c.Int(nullable: false),
                        VoteStatus = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.mobSocial_VideoBattle", t => t.VideoBattleId, cascadeDelete: true)
                .Index(t => t.VideoBattleId);
            
            CreateTable(
                "dbo.mobSocial_VoterPass",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        BattleId = c.Int(nullable: false),
                        BattleType = c.Int(nullable: false),
                        VoterPassOrderId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.mobSocial_ArtistPagePayment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ArtistPageId = c.Int(nullable: false),
                        PaymentType = c.Int(nullable: false),
                        PaypalEmail = c.String(),
                        BankName = c.String(),
                        RoutingNumber = c.String(),
                        AccountNumber = c.String(),
                        PayableTo = c.String(),
                        Address = c.String(),
                        City = c.String(),
                        Country = c.String(),
                        PostalCode = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.mobSocial_ArtistPage", t => t.ArtistPageId, cascadeDelete: true)
                .Index(t => t.ArtistPageId);
            
            CreateTable(
                "dbo.mobSocial_Address",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Address1 = c.String(),
                        Address2 = c.String(),
                        StateProvinceId = c.Int(nullable: false),
                        City = c.String(),
                        State = c.String(),
                        ZipPostalCode = c.String(),
                        CountryId = c.Int(nullable: false),
                        Phone = c.String(),
                        Website = c.String(),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.mobSocial_EntityAddress",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EntityId = c.Int(nullable: false),
                        EntityName = c.String(),
                        AddressId = c.Int(nullable: false),
                        IsDefault = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);

            //drop and recreate constraints if required
            var isUserIntegrated = userIntegrationMap.SourceTableName != "mobSocial_User";
            if (isUserIntegrated)
            {
                PostInstallationTasks.RunPostInstallation(() =>
                {
                    using (mobSocialEngine.ActiveEngine.IocContainer.OpenScope(Reuse.WebRequestScopeName))
                    {
                        var dbContext = mobSocialEngine.ActiveEngine.Resolve<IDatabaseContext>();
                        var dropConstraintString = @"ALTER TABLE {0} DROP CONSTRAINT [FK_{1}_{2}_{3}]";
                        Action<string> executeSql = sql =>
                        {
                            dbContext.ExecuteSql(TransactionalBehavior.EnsureTransaction, sql);
                        };

                        executeSql(string.Format(dropConstraintString, "dbo.mobSocial_Conversation", "dbo.mobSocial_Conversation", "dbo.mobSocial_User", "UserId"));
                        executeSql(string.Format(dropConstraintString, "dbo.mobSocial_ConversationReply", "dbo.mobSocial_ConversationReply", "dbo.mobSocial_User", "UserId"));
                        executeSql(string.Format(dropConstraintString, "dbo.mobSocial_Notification", "dbo.mobSocial_Notification", "dbo.mobSocial_User", "UserId"));
                        executeSql(string.Format(dropConstraintString, "dbo.mobSocial_UserSkill", "dbo.mobSocial_UserSkill", "dbo.mobSocial_User", "UserId"));
                        executeSql(string.Format(dropConstraintString, "dbo.mobSocial_UserRole", "dbo.mobSocial_UserRole", "dbo.mobSocial_User", "UserId"));
                        executeSql(string.Format(dropConstraintString, "dbo.mobSocial_Education", "dbo.mobSocial_Education", "dbo.mobSocial_User", "UserId"));

                        //add with new table
                        var constraintString = @"ALTER TABLE {0} ADD CONSTRAINT [FK_{1}] FOREIGN KEY ({2}) REFERENCES {3}({4});";
                        executeSql(string.Format(constraintString, "dbo.mobSocial_Conversation", "dbo.mobSocial_Conversation", "UserId", userIntegrationMap.SourceTableName, "Id"));
                        executeSql(string.Format(constraintString, "dbo.mobSocial_ConversationReply", "dbo.mobSocial_ConversationReply", "UserId", userIntegrationMap.SourceTableName, "Id"));
                        executeSql(string.Format(constraintString, "dbo.mobSocial_Notification", "dbo.mobSocial_Notification", "UserId", userIntegrationMap.SourceTableName, "Id"));
                        executeSql(string.Format(constraintString, "dbo.mobSocial_UserSkill", "dbo.mobSocial_UserSkill", "UserId", userIntegrationMap.SourceTableName, "Id"));
                        executeSql(string.Format(constraintString, "dbo.mobSocial_UserRole", "dbo.mobSocial_UserRole", "UserId", userIntegrationMap.SourceTableName, "Id"));
                        executeSql(string.Format(constraintString, "dbo.mobSocial_Education", "dbo.mobSocial_Education", "UserId", userIntegrationMap.SourceTableName, "Id"));
                    }
                    
                });
                
            }
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.mobSocial_ArtistPagePayment", "ArtistPageId", "dbo.mobSocial_ArtistPage");
            DropForeignKey("dbo.mobSocial_VideoBattleVote", "VideoBattleId", "dbo.mobSocial_VideoBattle");
            DropForeignKey("dbo.mobSocial_VideoBattleVideo", "VideoBattleId", "dbo.mobSocial_VideoBattle");
            DropForeignKey("dbo.mobSocial_VideoBattleParticipant", "VideoBattleId", "dbo.mobSocial_VideoBattle");
            DropForeignKey("dbo.mobSocial_VideoBattleGenre", "VideoGenreId", "dbo.mobSocial_VideoGenre");
            DropForeignKey("dbo.mobSocial_VideoBattleGenre", "VideoBattleId", "dbo.mobSocial_VideoBattle");
            DropForeignKey("dbo.mobSocial_VideoBattlePrize", "VideoBattleId", "dbo.mobSocial_VideoBattle");
            DropForeignKey("dbo.mobSocial_BusinessPageCoupon", "BusinessPageId", "dbo.mobSocial_BusinessPage");
            DropForeignKey("dbo.mobSocial_Conversation", "UserId", "dbo.mobSocial_User");
            DropForeignKey("dbo.mobSocial_ConversationReply", "UserId", "dbo.mobSocial_User");
            DropForeignKey("dbo.mobSocial_ConversationReplyStatus", "ReplyId", "dbo.mobSocial_ConversationReply");
            DropForeignKey("dbo.mobSocial_ConversationReply", "ConversationId", "dbo.mobSocial_Conversation");
            DropForeignKey("dbo.mobSocial_Credit", "PaymentTransactionId", "dbo.mobSocial_PaymentTransaction");
            DropForeignKey("dbo.mobSocial_EmailTemplate", "ParentEmailTemplateId", "dbo.mobSocial_EmailTemplate");
            DropForeignKey("dbo.mobSocial_EmailTemplate", "EmailAccountId", "dbo.mobSocial_EmailAccount");
            DropForeignKey("dbo.mobSocial_EmailMessage", "EmailAccountId", "dbo.mobSocial_EmailAccount");
            DropForeignKey("dbo.mobSocial_Notification", "UserId", "dbo.mobSocial_User");
            DropForeignKey("dbo.mobSocial_Notification", "NotificationEventId", "dbo.mobSocial_NotificationEvent");
            DropForeignKey("dbo.mobSocial_UserSkill", "UserId", "dbo.mobSocial_User");
            DropForeignKey("dbo.mobSocial_UserSkill", "SkillId", "dbo.mobSocial_Skill");
            DropForeignKey("dbo.mobSocial_EventPageAttendance", "EventPageId", "dbo.mobSocial_EventPage");
            DropForeignKey("dbo.mobSocial_EventPageHotel", "EventPageId", "dbo.mobSocial_EventPage");
            DropForeignKey("dbo.mobSocial_SharedSong", "SongId", "dbo.mobSocial_Song");
            DropForeignKey("dbo.mobSocial_Song", "ArtistPageId", "dbo.mobSocial_ArtistPage");
            DropForeignKey("dbo.mobSocial_ArtistPageManager", "ArtistPageId", "dbo.mobSocial_ArtistPage");
            DropForeignKey("dbo.mobSocial_GroupPage", "TeamPageId", "dbo.mobSocial_TeamPage");
            DropForeignKey("dbo.mobSocial_GroupPageMember", "GroupPageId", "dbo.mobSocial_GroupPage");
            DropForeignKey("dbo.mobSocial_UserRole", "UserId", "dbo.mobSocial_User");
            DropForeignKey("dbo.mobSocial_UserRole", "RoleId", "dbo.mobSocial_Role");
            DropForeignKey("dbo.mobSocial_Education", "UserId", "dbo.mobSocial_User");
            DropForeignKey("dbo.mobSocial_Education", "SchoolId", "dbo.mobSocial_School");
            DropForeignKey("dbo.mobSocial_RoleCapability", "RoleId", "dbo.mobSocial_Role");
            DropForeignKey("dbo.mobSocial_RoleCapability", "CapabilityId", "dbo.mobSocial_Capability");
            DropIndex("dbo.mobSocial_ArtistPagePayment", new[] { "ArtistPageId" });
            DropIndex("dbo.mobSocial_VideoBattleVote", new[] { "VideoBattleId" });
            DropIndex("dbo.mobSocial_VideoBattleVideo", new[] { "VideoBattleId" });
            DropIndex("dbo.mobSocial_VideoBattleParticipant", new[] { "VideoBattleId" });
            DropIndex("dbo.mobSocial_VideoBattleGenre", new[] { "VideoGenreId" });
            DropIndex("dbo.mobSocial_VideoBattleGenre", new[] { "VideoBattleId" });
            DropIndex("dbo.mobSocial_VideoBattlePrize", new[] { "VideoBattleId" });
            DropIndex("dbo.mobSocial_BusinessPageCoupon", new[] { "BusinessPageId" });
            DropIndex("dbo.mobSocial_ConversationReplyStatus", new[] { "ReplyId" });
            DropIndex("dbo.mobSocial_ConversationReply", new[] { "UserId" });
            DropIndex("dbo.mobSocial_ConversationReply", new[] { "ConversationId" });
            DropIndex("dbo.mobSocial_Conversation", new[] { "UserId" });
            DropIndex("dbo.mobSocial_Credit", new[] { "PaymentTransactionId" });
            DropIndex("dbo.mobSocial_EmailTemplate", new[] { "EmailAccountId" });
            DropIndex("dbo.mobSocial_EmailTemplate", new[] { "ParentEmailTemplateId" });
            DropIndex("dbo.mobSocial_EmailMessage", new[] { "EmailAccountId" });
            DropIndex("dbo.mobSocial_Notification", new[] { "NotificationEventId" });
            DropIndex("dbo.mobSocial_Notification", new[] { "UserId" });
            DropIndex("dbo.mobSocial_UserSkill", new[] { "UserId" });
            DropIndex("dbo.mobSocial_UserSkill", new[] { "SkillId" });
            DropIndex("dbo.mobSocial_EventPageAttendance", new[] { "EventPageId" });
            DropIndex("dbo.mobSocial_EventPageHotel", new[] { "EventPageId" });
            DropIndex("dbo.mobSocial_ArtistPageManager", new[] { "ArtistPageId" });
            DropIndex("dbo.mobSocial_Song", new[] { "ArtistPageId" });
            DropIndex("dbo.mobSocial_SharedSong", new[] { "SongId" });
            DropIndex("dbo.mobSocial_GroupPageMember", new[] { "GroupPageId" });
            DropIndex("dbo.mobSocial_GroupPage", new[] { "TeamPageId" });
            DropIndex("dbo.mobSocial_UserRole", new[] { "RoleId" });
            DropIndex("dbo.mobSocial_UserRole", new[] { "UserId" });
            DropIndex("dbo.mobSocial_Education", new[] { "SchoolId" });
            DropIndex("dbo.mobSocial_Education", new[] { "UserId" });
            DropIndex("dbo.mobSocial_RoleCapability", new[] { "CapabilityId" });
            DropIndex("dbo.mobSocial_RoleCapability", new[] { "RoleId" });
            DropTable("dbo.mobSocial_EntityAddress");
            DropTable("dbo.mobSocial_Address");
            DropTable("dbo.mobSocial_ArtistPagePayment");
            DropTable("dbo.mobSocial_VoterPass");
            DropTable("dbo.mobSocial_VideoBattleVote");
            DropTable("dbo.mobSocial_VideoBattleView");
            DropTable("dbo.mobSocial_VideoBattleVideo");
            DropTable("dbo.mobSocial_VideoBattleParticipant");
            DropTable("dbo.mobSocial_VideoBattleGenre");
            DropTable("dbo.mobSocial_VideoBattlePrize");
            DropTable("dbo.mobSocial_VideoBattle");
            DropTable("dbo.mobSocial_BusinessPageCoupon");
            DropTable("dbo.mobSocial_BusinessPage");
            DropTable("dbo.mobSocial_ConversationReplyStatus");
            DropTable("dbo.mobSocial_ConversationReply");
            DropTable("dbo.mobSocial_Conversation");
            DropTable("dbo.mobSocial_Credit");
            DropTable("dbo.mobSocial_Currency");
            DropTable("dbo.mobSocial_EmailTemplate");
            DropTable("dbo.mobSocial_EmailMessage");
            DropTable("dbo.mobSocial_EmailAccount");
            DropTable("dbo.mobSocial_NotificationEvent");
            DropTable("dbo.mobSocial_Notification");
            DropTable("dbo.mobSocial_ScheduledTask");
            DropTable("dbo.mobSocial_Setting");
            DropTable("dbo.mobSocial_UserSkill");
            DropTable("dbo.mobSocial_Skill");
            DropTable("dbo.mobSocial_EntityProperty");
            DropTable("dbo.mobSocial_EventPageAttendance");
            DropTable("dbo.mobSocial_EventPageHotel");
            DropTable("dbo.mobSocial_EventPage");
            DropTable("dbo.mobSocial_PaymentTransaction");
            DropTable("dbo.mobSocial_UserPaymentMethod");
            DropTable("dbo.mobSocial_Media");
            DropTable("dbo.mobSocial_EntityMedia");
            DropTable("dbo.mobSocial_Permalink");
            DropTable("dbo.mobSocial_UserLike");
            DropTable("dbo.mobSocial_Friend");
            DropTable("dbo.mobSocial_UserFollow");
            DropTable("dbo.mobSocial_Comment");
            DropTable("dbo.mobSocial_ArtistPageManager");
            DropTable("dbo.mobSocial_ArtistPage");
            DropTable("dbo.mobSocial_Song");
            DropTable("dbo.mobSocial_SharedSong");
            DropTable("dbo.mobSocial_SponsorPass");
            DropTable("dbo.mobSocial_SponsorData");
            DropTable("dbo.mobSocial_Sponsor");
            DropTable("dbo.mobSocial_GroupPageMember");
            DropTable("dbo.mobSocial_GroupPage");
            DropTable("dbo.mobSocial_TeamPage");
            DropTable("dbo.mobSocial_TimelinePost");
            DropTable("dbo.mobSocial_UserRole");
            DropTable("dbo.mobSocial_School");
            DropTable("dbo.mobSocial_Education");
            DropTable("dbo.mobSocial_User");
            DropTable("dbo.mobSocial_Role");
            DropTable("dbo.mobSocial_Capability");
            DropTable("dbo.mobSocial_RoleCapability");
            DropTable("dbo.mobSocial_WatchedVideo");
            DropTable("dbo.mobSocial_VideoGenre");
            DropTable("dbo.mobSocial_CustomerVideo");
        }
    }
}
