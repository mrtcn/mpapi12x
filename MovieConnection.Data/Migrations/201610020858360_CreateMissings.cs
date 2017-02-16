namespace MovieConnections.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateMissings : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Movie", "Year", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Movie", "Year", c => c.DateTime(nullable: false));
        }
    }
}
