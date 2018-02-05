using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GenesisVision.Tournament.Core.Migrations
{
    public partial class ParticipantInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "Participants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EthAddress",
                table: "Participants",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "EthAddress",
                table: "Participants");
        }
    }
}
