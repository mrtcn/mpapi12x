namespace MovieConnections.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DataContext : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ActorMovie", "IsStar", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ActorMovie", "IsStar");
        }
    }
}
