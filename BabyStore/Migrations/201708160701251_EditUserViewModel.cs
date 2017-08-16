namespace BabyStore.Migrations.StoreConfiguration
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditUserViewModel : DbMigration
    {
        public override void Up()
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
            
        }
        
        public override void Down()
        {
            DropTable("dbo.EditUserViewModels");
        }
    }
}
