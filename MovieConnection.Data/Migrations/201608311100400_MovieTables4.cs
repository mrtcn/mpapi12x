namespace MovieConnections.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MovieTables4 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.CategoryMovie", newName: "GenreMovie");
            RenameTable(name: "dbo.Category", newName: "Genre");
            RenameTable(name: "dbo.CategoryCulture", newName: "GenreCulture");
            RenameColumn(table: "dbo.GenreMovie", name: "CategoryId", newName: "GenreId");
            RenameIndex(table: "dbo.GenreMovie", name: "IX_CategoryId", newName: "IX_GenreId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.GenreMovie", name: "IX_GenreId", newName: "IX_CategoryId");
            RenameColumn(table: "dbo.GenreMovie", name: "GenreId", newName: "CategoryId");
            RenameTable(name: "dbo.GenreCulture", newName: "CategoryCulture");
            RenameTable(name: "dbo.Genre", newName: "Category");
            RenameTable(name: "dbo.GenreMovie", newName: "CategoryMovie");
        }
    }
}
