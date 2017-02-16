namespace MovieConnections.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MovieTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CategoryMovie",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MovieId = c.Int(nullable: false),
                        CategoryId = c.Int(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                        UserType = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Category", t => t.CategoryId, cascadeDelete: true)
                .ForeignKey("dbo.Movie", t => t.MovieId, cascadeDelete: true)
                .Index(t => t.MovieId)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.Category",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                        UserType = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CategoryCulture",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BaseEntityId = c.Int(nullable: false),
                        CultureId = c.Int(nullable: false),
                        Name = c.String(),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                        UserType = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Category", t => t.BaseEntityId, cascadeDelete: true)
                .ForeignKey("dbo.Culture", t => t.CultureId, cascadeDelete: true)
                .Index(t => t.BaseEntityId)
                .Index(t => t.CultureId);
            
            CreateTable(
                "dbo.Movie",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OriginalTitle = c.String(),
                        Director = c.String(),
                        Rating = c.Double(nullable: false),
                        NumberOfVotes = c.Int(nullable: false),
                        Year = c.DateTime(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                        UserType = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ActorMovie",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MovieId = c.Int(nullable: false),
                        ActorId = c.Int(nullable: false),
                        CharacterName = c.String(),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                        UserType = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Actor", t => t.ActorId, cascadeDelete: true)
                .ForeignKey("dbo.Movie", t => t.MovieId, cascadeDelete: true)
                .Index(t => t.MovieId)
                .Index(t => t.ActorId);
            
            CreateTable(
                "dbo.Actor",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                        UserType = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ActorCulture",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BaseEntityId = c.Int(nullable: false),
                        CultureId = c.Int(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                        UserType = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Actor", t => t.BaseEntityId, cascadeDelete: true)
                .ForeignKey("dbo.Culture", t => t.CultureId, cascadeDelete: true)
                .Index(t => t.BaseEntityId)
                .Index(t => t.CultureId);
            
            CreateTable(
                "dbo.MovieCulture",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BaseEntityId = c.Int(nullable: false),
                        CultureId = c.Int(nullable: false),
                        Title = c.String(),
                        Country = c.String(),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                        UserType = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Movie", t => t.BaseEntityId, cascadeDelete: true)
                .ForeignKey("dbo.Culture", t => t.CultureId, cascadeDelete: true)
                .Index(t => t.BaseEntityId)
                .Index(t => t.CultureId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CategoryMovie", "MovieId", "dbo.Movie");
            DropForeignKey("dbo.MovieCulture", "CultureId", "dbo.Culture");
            DropForeignKey("dbo.MovieCulture", "BaseEntityId", "dbo.Movie");
            DropForeignKey("dbo.ActorMovie", "MovieId", "dbo.Movie");
            DropForeignKey("dbo.ActorMovie", "ActorId", "dbo.Actor");
            DropForeignKey("dbo.ActorCulture", "CultureId", "dbo.Culture");
            DropForeignKey("dbo.ActorCulture", "BaseEntityId", "dbo.Actor");
            DropForeignKey("dbo.CategoryMovie", "CategoryId", "dbo.Category");
            DropForeignKey("dbo.CategoryCulture", "CultureId", "dbo.Culture");
            DropForeignKey("dbo.CategoryCulture", "BaseEntityId", "dbo.Category");
            DropIndex("dbo.MovieCulture", new[] { "CultureId" });
            DropIndex("dbo.MovieCulture", new[] { "BaseEntityId" });
            DropIndex("dbo.ActorCulture", new[] { "CultureId" });
            DropIndex("dbo.ActorCulture", new[] { "BaseEntityId" });
            DropIndex("dbo.ActorMovie", new[] { "ActorId" });
            DropIndex("dbo.ActorMovie", new[] { "MovieId" });
            DropIndex("dbo.CategoryCulture", new[] { "CultureId" });
            DropIndex("dbo.CategoryCulture", new[] { "BaseEntityId" });
            DropIndex("dbo.CategoryMovie", new[] { "CategoryId" });
            DropIndex("dbo.CategoryMovie", new[] { "MovieId" });
            DropTable("dbo.MovieCulture");
            DropTable("dbo.ActorCulture");
            DropTable("dbo.Actor");
            DropTable("dbo.ActorMovie");
            DropTable("dbo.Movie");
            DropTable("dbo.CategoryCulture");
            DropTable("dbo.Category");
            DropTable("dbo.CategoryMovie");
        }
    }
}
