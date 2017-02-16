namespace MovieConnections.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConnectionPathCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ConnectionPath",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BaseActorId = c.Int(nullable: false),
                        DestinationActorId = c.Int(nullable: false),
                        ActorPath = c.String(),
                        MoviePath = c.String(),
                        MaxBranchLevel = c.Int(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                        UserType = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Actor", t => t.BaseActorId, cascadeDelete: true)
                .Index(t => t.BaseActorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ConnectionPath", "BaseActorId", "dbo.Actor");
            DropIndex("dbo.ConnectionPath", new[] { "BaseActorId" });
            DropTable("dbo.ConnectionPath");
        }
    }
}
