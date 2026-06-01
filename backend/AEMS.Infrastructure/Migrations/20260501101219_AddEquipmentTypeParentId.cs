using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AEMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipmentTypeParentId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "equipment_type",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_equipment_type_ParentId",
                table: "equipment_type",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_equipment_type_equipment_type_ParentId",
                table: "equipment_type",
                column: "ParentId",
                principalTable: "equipment_type",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_equipment_type_equipment_type_ParentId",
                table: "equipment_type");

            migrationBuilder.DropIndex(
                name: "IX_equipment_type_ParentId",
                table: "equipment_type");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "equipment_type");
        }
    }
}
