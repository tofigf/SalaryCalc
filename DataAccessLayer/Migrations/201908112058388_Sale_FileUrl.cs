namespace DataAccessLayer
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Sale_FileUrl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sales", "FileUrl", c => c.String(maxLength: 250));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sales", "FileUrl");
        }
    }
}
