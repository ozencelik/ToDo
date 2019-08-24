namespace ToDoList.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ToDo1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ToDoes", "Deadline", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ToDoes", "Deadline");
        }
    }
}
