using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AEMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSubsystemType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubsystemType",
                table: "subsystem");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubsystemType",
                table: "subsystem",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
