using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class EnsuringTransactionsUseDoubleEntryBookkeeping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LedgerEntry_UniTreeGroups_StokvelGroupId",
                table: "LedgerEntry");

            migrationBuilder.DropForeignKey(
                name: "FK_LedgerEntry_Wallets_WalletId",
                table: "LedgerEntry");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "LedgerEntry");

            migrationBuilder.AlterColumn<int>(
                name: "WalletId",
                table: "LedgerEntry",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "StokvelGroupId",
                table: "LedgerEntry",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_LedgerEntry_UniTreeGroups_StokvelGroupId",
                table: "LedgerEntry",
                column: "StokvelGroupId",
                principalTable: "UniTreeGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LedgerEntry_Wallets_WalletId",
                table: "LedgerEntry",
                column: "WalletId",
                principalTable: "Wallets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LedgerEntry_UniTreeGroups_StokvelGroupId",
                table: "LedgerEntry");

            migrationBuilder.DropForeignKey(
                name: "FK_LedgerEntry_Wallets_WalletId",
                table: "LedgerEntry");

            migrationBuilder.AlterColumn<int>(
                name: "WalletId",
                table: "LedgerEntry",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StokvelGroupId",
                table: "LedgerEntry",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TransactionId",
                table: "LedgerEntry",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddForeignKey(
                name: "FK_LedgerEntry_UniTreeGroups_StokvelGroupId",
                table: "LedgerEntry",
                column: "StokvelGroupId",
                principalTable: "UniTreeGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LedgerEntry_Wallets_WalletId",
                table: "LedgerEntry",
                column: "WalletId",
                principalTable: "Wallets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
