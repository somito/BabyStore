namespace BabyStore.Migrations.StoreConfiguration
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBasketLine : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BasketLines",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        BasketID = c.String(),
                        ProductID = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Products", t => t.ProductID, cascadeDelete: true)
                .Index(t => t.ProductID);
            
            DropTable("dbo.EditUserViewModels");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.EditUserViewModels",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(nullable: false),
                        FirstName = c.String(nullable: false, maxLength: 50),
                        LastName = c.String(nullable: false, maxLength: 50),
                        DateOfBirth = c.DateTime(nullable: false),
                        Address_AddressLine1 = c.String(nullable: false),
                        Address_AddressLine2 = c.String(),
                        Address_Town = c.String(nullable: false),
                        Address_County = c.String(nullable: false),
                        Address_Postcode = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.BasketLines", "ProductID", "dbo.Products");
            DropIndex("dbo.BasketLines", new[] { "ProductID" });
            DropTable("dbo.BasketLines");
        }
    }
}
