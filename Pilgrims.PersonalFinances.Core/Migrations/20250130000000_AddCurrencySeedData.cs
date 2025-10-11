using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Pilgrims.PersonalFinances.Core.Data;

#nullable disable

namespace Pilgrims.PersonalFinances.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencySeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var currencies = CurrencySeedData.GetCurrencies();
            
            foreach (var currency in currencies)
            {
                migrationBuilder.InsertData(
                    table: "Currencies",
                    columns: new[] { "Id", "Country", "CurrencyName", "ISOCode", "SymbolOrSign", "Notes", "CreatedAt", "UpdatedAt", "IsDirty" },
                    values: new object[] { 
                        currency.Id, 
                        currency.Country, 
                        currency.CurrencyName, 
                        currency.ISOCode, 
                        currency.SymbolOrSign ?? "", 
                        currency.Notes ?? "", 
                        currency.CreatedAt, 
                        currency.UpdatedAt, 
                        currency.IsDirty 
                    });
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Id",
                keyValues: CurrencySeedData.GetCurrencies().Select(c => c.Id).ToArray());
        }
    }
}