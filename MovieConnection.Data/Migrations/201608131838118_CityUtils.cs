namespace DefaultCulturedProject.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CityUtils : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CityCulture", "City_Id", "dbo.City");
            DropIndex("dbo.CityCulture", new[] { "City_Id" });
            DropColumn("dbo.CityCulture", "BaseEntityId");
            RenameColumn(table: "dbo.CityCulture", name: "City_Id", newName: "BaseEntityId");
            AlterColumn("dbo.CityCulture", "BaseEntityId", c => c.Int(nullable: false));
            CreateIndex("dbo.CityCulture", "BaseEntityId");
            AddForeignKey("dbo.CityCulture", "BaseEntityId", "dbo.City", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CityCulture", "BaseEntityId", "dbo.City");
            DropIndex("dbo.CityCulture", new[] { "BaseEntityId" });
            AlterColumn("dbo.CityCulture", "BaseEntityId", c => c.Int());
            RenameColumn(table: "dbo.CityCulture", name: "BaseEntityId", newName: "City_Id");
            AddColumn("dbo.CityCulture", "BaseEntityId", c => c.Int(nullable: false));
            CreateIndex("dbo.CityCulture", "City_Id");
            AddForeignKey("dbo.CityCulture", "City_Id", "dbo.City", "Id");
        }
    }
}
