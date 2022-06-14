namespace FarmCentral.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialDatabaseCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        Username = c.String(nullable: false, maxLength: 128),
                        Password = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Username);
            
            CreateTable(
                "dbo.Farmer_Product",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FarmerUsername = c.String(nullable: false, maxLength: 128),
                        ProductName = c.String(nullable: false, maxLength: 128),
                        Quantity = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Farmers", t => t.FarmerUsername, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductName, cascadeDelete: true)
                .Index(t => t.FarmerUsername)
                .Index(t => t.ProductName);
            
            CreateTable(
                "dbo.Farmers",
                c => new
                    {
                        Username = c.String(nullable: false, maxLength: 128),
                        Password = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Username);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Name);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Farmer_Product", "ProductName", "dbo.Products");
            DropForeignKey("dbo.Farmer_Product", "FarmerUsername", "dbo.Farmers");
            DropIndex("dbo.Farmer_Product", new[] { "ProductName" });
            DropIndex("dbo.Farmer_Product", new[] { "FarmerUsername" });
            DropTable("dbo.Products");
            DropTable("dbo.Farmers");
            DropTable("dbo.Farmer_Product");
            DropTable("dbo.Employees");
        }
    }
}
