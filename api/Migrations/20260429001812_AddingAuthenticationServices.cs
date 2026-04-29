using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class AddingAuthenticationServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LedgerEntry_Transactions_TransactionsId",
                table: "LedgerEntry");

            migrationBuilder.DropForeignKey(
                name: "FK_LedgerEntry_UniTreeGroups_StokvelGroupId",
                table: "LedgerEntry");

            migrationBuilder.DropForeignKey(
                name: "FK_LedgerEntry_Wallets_WalletId",
                table: "LedgerEntry");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LedgerEntry",
                table: "LedgerEntry");

            migrationBuilder.RenameTable(
                name: "LedgerEntry",
                newName: "LedgerEntries");

            migrationBuilder.RenameIndex(
                name: "IX_LedgerEntry_WalletId",
                table: "LedgerEntries",
                newName: "IX_LedgerEntries_WalletId");

            migrationBuilder.RenameIndex(
                name: "IX_LedgerEntry_TransactionsId",
                table: "LedgerEntries",
                newName: "IX_LedgerEntries_TransactionsId");

            migrationBuilder.RenameIndex(
                name: "IX_LedgerEntry_StokvelGroupId",
                table: "LedgerEntries",
                newName: "IX_LedgerEntries_StokvelGroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LedgerEntries",
                table: "LedgerEntries",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LedgerEntries_Transactions_TransactionsId",
                table: "LedgerEntries",
                column: "TransactionsId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LedgerEntries_UniTreeGroups_StokvelGroupId",
                table: "LedgerEntries",
                column: "StokvelGroupId",
                principalTable: "UniTreeGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LedgerEntries_Wallets_WalletId",
                table: "LedgerEntries",
                column: "WalletId",
                principalTable: "Wallets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LedgerEntries_Transactions_TransactionsId",
                table: "LedgerEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_LedgerEntries_UniTreeGroups_StokvelGroupId",
                table: "LedgerEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_LedgerEntries_Wallets_WalletId",
                table: "LedgerEntries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LedgerEntries",
                table: "LedgerEntries");

            migrationBuilder.RenameTable(
                name: "LedgerEntries",
                newName: "LedgerEntry");

            migrationBuilder.RenameIndex(
                name: "IX_LedgerEntries_WalletId",
                table: "LedgerEntry",
                newName: "IX_LedgerEntry_WalletId");

            migrationBuilder.RenameIndex(
                name: "IX_LedgerEntries_TransactionsId",
                table: "LedgerEntry",
                newName: "IX_LedgerEntry_TransactionsId");

            migrationBuilder.RenameIndex(
                name: "IX_LedgerEntries_StokvelGroupId",
                table: "LedgerEntry",
                newName: "IX_LedgerEntry_StokvelGroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LedgerEntry",
                table: "LedgerEntry",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LedgerEntry_Transactions_TransactionsId",
                table: "LedgerEntry",
                column: "TransactionsId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
    }
}
