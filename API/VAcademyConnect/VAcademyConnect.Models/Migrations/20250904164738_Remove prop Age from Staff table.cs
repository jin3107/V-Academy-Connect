using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VAcademyConnect.Models.Migrations
{
    /// <inheritdoc />
    public partial class RemovepropAgefromStafftable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "Staffs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "Staffs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
