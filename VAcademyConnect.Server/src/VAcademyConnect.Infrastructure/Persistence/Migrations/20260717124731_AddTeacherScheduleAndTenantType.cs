using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VAcademyConnect.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTeacherScheduleAndTenantType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Tenants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "TeacherId",
                table: "Classes",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<int>(
                name: "TeachingDayOfWeek",
                table: "Classes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "TeachingEndTime",
                table: "Classes",
                type: "time(6)",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "TeachingStartTime",
                table: "Classes",
                type: "time(6)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Classes_TeacherId",
                table: "Classes",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Staffs_TeacherId",
                table: "Classes",
                column: "TeacherId",
                principalTable: "Staffs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Staffs_TeacherId",
                table: "Classes");

            migrationBuilder.DropIndex(
                name: "IX_Classes_TeacherId",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "TeachingDayOfWeek",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "TeachingEndTime",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "TeachingStartTime",
                table: "Classes");
        }
    }
}
