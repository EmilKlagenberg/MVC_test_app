using Microsoft.EntityFrameworkCore.Migrations;

namespace MVC_test_app.Migrations
{
    public partial class SecurityScaffoldFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "Item",
            //    table: "AspNetUsers");

            //migrationBuilder.AddColumn<string>(
            //    name: "CustomTag",
            //    table: "AspNetUsers",
            //    nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "CustomTag",
            //    table: "AspNetUsers");

            //migrationBuilder.AddColumn<string>(
            //    name: "Item",
            //    table: "AspNetUsers",
            //    type: "TEXT",
            //    nullable: true);
        }
    }
}
