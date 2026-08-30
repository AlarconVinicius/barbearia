using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberFlow.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "inbox_messages",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                message_id = table.Column<Guid>(type: "uuid", nullable: false),
                consumer = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                processed_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_inbox_messages", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "outbox_messages",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                occurred_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                type = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                payload = table.Column<string>(type: "jsonb", nullable: false),
                correlation_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                processed_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                attempt_count = table.Column<int>(type: "integer", nullable: false),
                last_attempt_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                last_error = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_outbox_messages", x => x.id);
                table.CheckConstraint("ck_outbox_messages_attempt_count", "attempt_count >= 0");
            });

        migrationBuilder.CreateTable(
            name: "services",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                duration_minutes = table.Column<int>(type: "integer", nullable: false),
                is_active = table.Column<bool>(type: "boolean", nullable: false),
                created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_services", x => x.id);
                table.CheckConstraint("ck_services_duration", "duration_minutes > 0");
                table.CheckConstraint("ck_services_price", "price >= 0");
            });

        migrationBuilder.CreateTable(
            name: "users",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                full_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                phone_number = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                cpf = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                is_active = table.Column<bool>(type: "boolean", nullable: false),
                created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_users", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "appointments",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                starts_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                ends_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                status = table.Column<int>(type: "integer", nullable: false),
                created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                cancelled_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                cancelled_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                cancellation_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_appointments", x => x.id);
                table.CheckConstraint("ck_appointments_different_users", "customer_id <> employee_id");
                table.CheckConstraint("ck_appointments_time_range", "starts_at_utc < ends_at_utc");
                table.ForeignKey(
                    name: "FK_appointments_users_cancelled_by_user_id",
                    column: x => x.cancelled_by_user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.SetNull);
                table.ForeignKey(
                    name: "FK_appointments_users_created_by_user_id",
                    column: x => x.created_by_user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_appointments_users_customer_id",
                    column: x => x.customer_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_appointments_users_employee_id",
                    column: x => x.employee_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "audit_entries",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                occurred_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: true),
                action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                entity_type = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                entity_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                correlation_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                data = table.Column<string>(type: "jsonb", nullable: true),
                created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_audit_entries", x => x.id);
                table.ForeignKey(
                    name: "FK_audit_entries_users_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateTable(
            name: "authentication_codes",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                purpose = table.Column<int>(type: "integer", nullable: false),
                code_hash = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                expires_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                attempt_count = table.Column<int>(type: "integer", nullable: false),
                used_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                invalidated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                locked_until_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_authentication_codes", x => x.id);
                table.CheckConstraint("ck_authentication_codes_attempt_count", "attempt_count >= 0 AND attempt_count <= 3");
                table.ForeignKey(
                    name: "FK_authentication_codes_users_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "employee_services",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                service_id = table.Column<Guid>(type: "uuid", nullable: false),
                created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_employee_services", x => x.id);
                table.ForeignKey(
                    name: "FK_employee_services_services_service_id",
                    column: x => x.service_id,
                    principalTable: "services",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_employee_services_users_employee_id",
                    column: x => x.employee_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "user_roles",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                role = table.Column<int>(type: "integer", nullable: false),
                created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_user_roles", x => x.id);
                table.ForeignKey(
                    name: "FK_user_roles_users_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "working_intervals",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                day_of_week = table.Column<int>(type: "integer", nullable: false),
                starts_at = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                ends_at = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_working_intervals", x => x.id);
                table.CheckConstraint("ck_working_intervals_time_range", "starts_at < ends_at");
                table.ForeignKey(
                    name: "FK_working_intervals_users_employee_id",
                    column: x => x.employee_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "appointment_items",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                appointment_id = table.Column<Guid>(type: "uuid", nullable: false),
                service_id = table.Column<Guid>(type: "uuid", nullable: false),
                service_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                unit_price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                duration_minutes = table.Column<int>(type: "integer", nullable: false),
                created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_appointment_items", x => x.id);
                table.CheckConstraint("ck_appointment_items_duration", "duration_minutes > 0");
                table.CheckConstraint("ck_appointment_items_unit_price", "unit_price >= 0");
                table.ForeignKey(
                    name: "FK_appointment_items_appointments_appointment_id",
                    column: x => x.appointment_id,
                    principalTable: "appointments",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_appointment_items_services_service_id",
                    column: x => x.service_id,
                    principalTable: "services",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "appointment_requests",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                idempotency_key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                requested_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                requested_starts_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                type = table.Column<int>(type: "integer", nullable: false),
                status = table.Column<int>(type: "integer", nullable: false),
                rejection_reason = table.Column<int>(type: "integer", nullable: true),
                rejection_details = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                appointment_id = table.Column<Guid>(type: "uuid", nullable: true),
                processed_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_appointment_requests", x => x.id);
                table.CheckConstraint("ck_appointment_requests_different_users", "customer_id <> employee_id");
                table.ForeignKey(
                    name: "FK_appointment_requests_appointments_appointment_id",
                    column: x => x.appointment_id,
                    principalTable: "appointments",
                    principalColumn: "id",
                    onDelete: ReferentialAction.SetNull);
                table.ForeignKey(
                    name: "FK_appointment_requests_users_customer_id",
                    column: x => x.customer_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_appointment_requests_users_employee_id",
                    column: x => x.employee_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_appointment_requests_users_requested_by_user_id",
                    column: x => x.requested_by_user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "ix_appointment_items_appointment_id",
            table: "appointment_items",
            column: "appointment_id");

        migrationBuilder.CreateIndex(
            name: "IX_appointment_items_service_id",
            table: "appointment_items",
            column: "service_id");

        migrationBuilder.CreateIndex(
            name: "IX_appointment_requests_appointment_id",
            table: "appointment_requests",
            column: "appointment_id");

        migrationBuilder.CreateIndex(
            name: "IX_appointment_requests_customer_id",
            table: "appointment_requests",
            column: "customer_id");

        migrationBuilder.CreateIndex(
            name: "ix_appointment_requests_employee_status_start",
            table: "appointment_requests",
            columns: new[] { "employee_id", "status", "requested_starts_at_utc" });

        migrationBuilder.CreateIndex(
            name: "ux_appointment_requests_requested_by_idempotency_key",
            table: "appointment_requests",
            columns: new[] { "requested_by_user_id", "idempotency_key" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_appointments_cancelled_by_user_id",
            table: "appointments",
            column: "cancelled_by_user_id");

        migrationBuilder.CreateIndex(
            name: "IX_appointments_created_by_user_id",
            table: "appointments",
            column: "created_by_user_id");

        migrationBuilder.CreateIndex(
            name: "ix_appointments_customer_start",
            table: "appointments",
            columns: new[] { "customer_id", "starts_at_utc" });

        migrationBuilder.CreateIndex(
            name: "ix_appointments_employee_status_start",
            table: "appointments",
            columns: new[] { "employee_id", "status", "starts_at_utc" });

        migrationBuilder.CreateIndex(
            name: "ix_audit_entries_occurred_at",
            table: "audit_entries",
            column: "occurred_at_utc");

        migrationBuilder.CreateIndex(
            name: "ix_audit_entries_user_id",
            table: "audit_entries",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_authentication_codes_user_purpose_created_at",
            table: "authentication_codes",
            columns: new[] { "user_id", "purpose", "created_at_utc" });

        migrationBuilder.CreateIndex(
            name: "IX_employee_services_service_id",
            table: "employee_services",
            column: "service_id");

        migrationBuilder.CreateIndex(
            name: "ux_employee_services_employee_id_service_id",
            table: "employee_services",
            columns: new[] { "employee_id", "service_id" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ux_inbox_messages_message_id_consumer",
            table: "inbox_messages",
            columns: new[] { "message_id", "consumer" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_outbox_messages_processed_at_occurred_at",
            table: "outbox_messages",
            columns: new[] { "processed_at_utc", "occurred_at_utc" });

        migrationBuilder.CreateIndex(
            name: "ix_services_name",
            table: "services",
            column: "name");

        migrationBuilder.CreateIndex(
            name: "ux_user_roles_user_id_role",
            table: "user_roles",
            columns: new[] { "user_id", "role" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ux_users_cpf",
            table: "users",
            column: "cpf",
            unique: true,
            filter: "cpf IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "ux_users_email",
            table: "users",
            column: "email",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_working_intervals_employee_id_day_of_week",
            table: "working_intervals",
            columns: new[] { "employee_id", "day_of_week" });

        migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS btree_gist;");

        migrationBuilder.Sql(
            """
            ALTER TABLE working_intervals
            ADD CONSTRAINT ex_working_intervals_no_overlap
            EXCLUDE USING gist
            (
                employee_id WITH =,
                day_of_week WITH =,
                numrange(
                    EXTRACT(EPOCH FROM starts_at),
                    EXTRACT(EPOCH FROM ends_at),
                    '[)') WITH &&
            );
            """);

        migrationBuilder.Sql(
            """
            ALTER TABLE appointments
            ADD CONSTRAINT ex_appointments_no_scheduled_overlap
            EXCLUDE USING gist
            (
                employee_id WITH =,
                tstzrange(starts_at_utc, ends_at_utc, '[)') WITH &&
            )
            WHERE (status = 1);
            """);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "appointment_items");

        migrationBuilder.DropTable(
            name: "appointment_requests");

        migrationBuilder.DropTable(
            name: "audit_entries");

        migrationBuilder.DropTable(
            name: "authentication_codes");

        migrationBuilder.DropTable(
            name: "employee_services");

        migrationBuilder.DropTable(
            name: "inbox_messages");

        migrationBuilder.DropTable(
            name: "outbox_messages");

        migrationBuilder.DropTable(
            name: "user_roles");

        migrationBuilder.DropTable(
            name: "working_intervals");

        migrationBuilder.DropTable(
            name: "appointments");

        migrationBuilder.DropTable(
            name: "services");

        migrationBuilder.DropTable(
            name: "users");
    }
}
