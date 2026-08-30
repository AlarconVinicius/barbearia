using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberFlow.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class RequireUserCpf : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "ux_users_cpf",
            table: "users");

        migrationBuilder.AlterColumn<string>(
            name: "cpf",
            table: "users",
            type: "character varying(11)",
            maxLength: 11,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(11)",
            oldMaxLength: 11,
            oldNullable: true);

        migrationBuilder.CreateIndex(
            name: "ux_users_cpf",
            table: "users",
            column: "cpf",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "ux_users_cpf",
            table: "users");

        migrationBuilder.AlterColumn<string>(
            name: "cpf",
            table: "users",
            type: "character varying(11)",
            maxLength: 11,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "character varying(11)",
            oldMaxLength: 11);

        migrationBuilder.CreateIndex(
            name: "ux_users_cpf",
            table: "users",
            column: "cpf",
            unique: true,
            filter: "cpf IS NOT NULL");
    }
}
