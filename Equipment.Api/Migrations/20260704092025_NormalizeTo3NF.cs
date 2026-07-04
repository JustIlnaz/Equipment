using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Equipment.Api.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeTo3NF : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResponsiblePerson",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Equipment");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "Equipment",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EquipmentStatusId",
                table: "Equipment",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EquipmentTypeId",
                table: "Equipment",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FullName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Role_id",
                table: "Users",
                column: "Role_id");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_EmployeeId",
                table: "Equipment",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_EquipmentStatusId",
                table: "Equipment",
                column: "EquipmentStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_EquipmentTypeId",
                table: "Equipment",
                column: "EquipmentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipment_Employees_EmployeeId",
                table: "Equipment",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipment_EquipmentStatuses_EquipmentStatusId",
                table: "Equipment",
                column: "EquipmentStatusId",
                principalTable: "EquipmentStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Equipment_EquipmentTypes_EquipmentTypeId",
                table: "Equipment",
                column: "EquipmentTypeId",
                principalTable: "EquipmentTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_Role_id",
                table: "Users",
                column: "Role_id",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipment_Employees_EmployeeId",
                table: "Equipment");

            migrationBuilder.DropForeignKey(
                name: "FK_Equipment_EquipmentStatuses_EquipmentStatusId",
                table: "Equipment");

            migrationBuilder.DropForeignKey(
                name: "FK_Equipment_EquipmentTypes_EquipmentTypeId",
                table: "Equipment");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_Role_id",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "EquipmentStatuses");

            migrationBuilder.DropTable(
                name: "EquipmentTypes");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Users_Role_id",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Equipment_EmployeeId",
                table: "Equipment");

            migrationBuilder.DropIndex(
                name: "IX_Equipment_EquipmentStatusId",
                table: "Equipment");

            migrationBuilder.DropIndex(
                name: "IX_Equipment_EquipmentTypeId",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "EquipmentStatusId",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "EquipmentTypeId",
                table: "Equipment");

            migrationBuilder.AddColumn<string>(
                name: "ResponsiblePerson",
                table: "Equipment",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Equipment",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Equipment",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
