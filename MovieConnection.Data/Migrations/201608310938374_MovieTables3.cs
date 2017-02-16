namespace MovieConnections.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MovieTables3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Movie", "MovieType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Movie", "MovieType");
        }
    }
}
