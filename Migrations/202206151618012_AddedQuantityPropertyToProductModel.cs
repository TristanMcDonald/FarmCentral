namespace FarmCentral.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedQuantityPropertyToProductModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "Quantity", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "Quantity");
        }
    }
}
