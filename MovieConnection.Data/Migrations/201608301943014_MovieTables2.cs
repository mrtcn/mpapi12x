namespace MovieConnections.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MovieTables2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ActorCulture", "BaseEntityId", "dbo.Actor");
            DropForeignKey("dbo.ActorCulture", "CultureId", "dbo.Culture");
            DropIndex("dbo.ActorCulture", new[] { "BaseEntityId" });
            DropIndex("dbo.ActorCulture", new[] { "CultureId" });
            DropTable("dbo.ActorCulture");
        }
        
        public override void Down()
        {
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
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.ActorCulture", "CultureId");
            CreateIndex("dbo.ActorCulture", "BaseEntityId");
            AddForeignKey("dbo.ActorCulture", "CultureId", "dbo.Culture", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ActorCulture", "BaseEntityId", "dbo.Actor", "Id", cascadeDelete: true);
        }
    }
}
