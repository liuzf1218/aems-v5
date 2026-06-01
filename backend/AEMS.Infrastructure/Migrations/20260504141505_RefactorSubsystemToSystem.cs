using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AEMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorSubsystemToSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_subsystem_equipment_EquipmentId",
                table: "subsystem");

            migrationBuilder.RenameColumn(
                name: "EquipmentId",
                table: "subsystem",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_subsystem_EquipmentId",
                table: "subsystem",
                newName: "IX_subsystem_CategoryId");

            migrationBuilder.AddColumn<int>(
                name: "SubsystemId",
                table: "equipment",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_equipment_SubsystemId",
                table: "equipment",
                column: "SubsystemId");

            migrationBuilder.AddForeignKey(
                name: "FK_equipment_subsystem_SubsystemId",
                table: "equipment",
                column: "SubsystemId",
                principalTable: "subsystem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_subsystem_equipment_type_CategoryId",
                table: "subsystem",
                column: "CategoryId",
                principalTable: "equipment_type",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_equipment_subsystem_SubsystemId",
                table: "equipment");

            migrationBuilder.DropForeignKey(
                name: "FK_subsystem_equipment_type_CategoryId",
                table: "subsystem");

            migrationBuilder.DropIndex(
                name: "IX_equipment_SubsystemId",
                table: "equipment");

            migrationBuilder.DropColumn(
                name: "SubsystemId",
                table: "equipment");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "subsystem",
                newName: "EquipmentId");

            migrationBuilder.RenameIndex(
                name: "IX_subsystem_CategoryId",
                table: "subsystem",
                newName: "IX_subsystem_EquipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_subsystem_equipment_EquipmentId",
                table: "subsystem",
                column: "EquipmentId",
                principalTable: "equipment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
