using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KolokwiumGrupaA.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Doctors",
            columns: table => new
            {
                DoctorId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Specialization = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Phone = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Doctors", x => x.DoctorId));

        migrationBuilder.CreateTable(
            name: "Medical_Services",
            columns: table => new
            {
                ServiceId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                DurationMinutes = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Medical_Services", x => x.ServiceId));

        migrationBuilder.CreateTable(
            name: "Patients",
            columns: table => new
            {
                PatientId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                Phone = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Patients", x => x.PatientId));

        migrationBuilder.CreateTable(
            name: "Appointments",
            columns: table => new
            {
                AppointmentId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                PatientId = table.Column<int>(type: "int", nullable: false),
                DoctorId = table.Column<int>(type: "int", nullable: false),
                AppointmentDate = table.Column<DateOnly>(type: "date", nullable: false),
                Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Appointments", x => x.AppointmentId);
                table.ForeignKey(
                    name: "FK_Appointments_Doctors_DoctorId",
                    column: x => x.DoctorId,
                    principalTable: "Doctors",
                    principalColumn: "DoctorId",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Appointments_Patients_PatientId",
                    column: x => x.PatientId,
                    principalTable: "Patients",
                    principalColumn: "PatientId",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Appointment_Services",
            columns: table => new
            {
                AppointmentId = table.Column<int>(type: "int", nullable: false),
                ServiceId = table.Column<int>(type: "int", nullable: false),
                Quantity = table.Column<int>(type: "int", nullable: false),
                PerformedAt = table.Column<DateOnly>(type: "date", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Appointment_Services", x => new { x.AppointmentId, x.ServiceId });
                table.ForeignKey(
                    name: "FK_Appointment_Services_Appointments_AppointmentId",
                    column: x => x.AppointmentId,
                    principalTable: "Appointments",
                    principalColumn: "AppointmentId",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Appointment_Services_Medical_Services_ServiceId",
                    column: x => x.ServiceId,
                    principalTable: "Medical_Services",
                    principalColumn: "ServiceId",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Appointment_Services_ServiceId",
            table: "Appointment_Services",
            column: "ServiceId");

        migrationBuilder.CreateIndex(
            name: "IX_Appointments_DoctorId",
            table: "Appointments",
            column: "DoctorId");

        migrationBuilder.CreateIndex(
            name: "IX_Appointments_PatientId",
            table: "Appointments",
            column: "PatientId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Appointment_Services");
        migrationBuilder.DropTable(name: "Appointments");
        migrationBuilder.DropTable(name: "Medical_Services");
        migrationBuilder.DropTable(name: "Patients");
        migrationBuilder.DropTable(name: "Doctors");
    }
}
