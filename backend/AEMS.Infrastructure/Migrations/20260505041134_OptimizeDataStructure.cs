using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AEMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OptimizeDataStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubsystemId",
                table: "sparepart",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EquipmentId",
                table: "software",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BuildingId",
                table: "room",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Floor",
                table: "room",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EquipmentId",
                table: "maintenance_plan",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoomId",
                table: "equipment",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "building",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_building", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sparepart_SubsystemId",
                table: "sparepart",
                column: "SubsystemId");

            migrationBuilder.CreateIndex(
                name: "IX_software_EquipmentId",
                table: "software",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_room_BuildingId",
                table: "room",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_maintenance_plan_EquipmentId",
                table: "maintenance_plan",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_equipment_RoomId",
                table: "equipment",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_building_Code",
                table: "building",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_equipment_room_RoomId",
                table: "equipment",
                column: "RoomId",
                principalTable: "room",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_maintenance_plan_equipment_EquipmentId",
                table: "maintenance_plan",
                column: "EquipmentId",
                principalTable: "equipment",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_room_building_BuildingId",
                table: "room",
                column: "BuildingId",
                principalTable: "building",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_software_equipment_EquipmentId",
                table: "software",
                column: "EquipmentId",
                principalTable: "equipment",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_sparepart_subsystem_SubsystemId",
                table: "sparepart",
                column: "SubsystemId",
                principalTable: "subsystem",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_equipment_room_RoomId",
                table: "equipment");

            migrationBuilder.DropForeignKey(
                name: "FK_maintenance_plan_equipment_EquipmentId",
                table: "maintenance_plan");

            migrationBuilder.DropForeignKey(
                name: "FK_room_building_BuildingId",
                table: "room");

            migrationBuilder.DropForeignKey(
                name: "FK_software_equipment_EquipmentId",
                table: "software");

            migrationBuilder.DropForeignKey(
                name: "FK_sparepart_subsystem_SubsystemId",
                table: "sparepart");

            migrationBuilder.DropTable(
                name: "building");

            migrationBuilder.DropIndex(
                name: "IX_sparepart_SubsystemId",
                table: "sparepart");

            migrationBuilder.DropIndex(
                name: "IX_software_EquipmentId",
                table: "software");

            migrationBuilder.DropIndex(
                name: "IX_room_BuildingId",
                table: "room");

            migrationBuilder.DropIndex(
                name: "IX_maintenance_plan_EquipmentId",
                table: "maintenance_plan");

            migrationBuilder.DropIndex(
                name: "IX_equipment_RoomId",
                table: "equipment");

            migrationBuilder.DropColumn(
                name: "SubsystemId",
                table: "sparepart");

            migrationBuilder.DropColumn(
                name: "EquipmentId",
                table: "software");

            migrationBuilder.DropColumn(
                name: "BuildingId",
                table: "room");

            migrationBuilder.DropColumn(
                name: "Floor",
                table: "room");

            migrationBuilder.DropColumn(
                name: "EquipmentId",
                table: "maintenance_plan");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "equipment");
        }
    }
}
