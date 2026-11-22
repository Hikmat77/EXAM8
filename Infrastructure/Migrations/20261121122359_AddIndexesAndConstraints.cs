using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexesAndConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex("IX_attendees_Email", "attendees", "Email", unique: true);
            migrationBuilder.CreateIndex("IX_categories_Name", "categories", "Name", unique: true);
            migrationBuilder.CreateIndex("IX_EventCategories_EventsId", "EventCategories", "EventsId");
            migrationBuilder.CreateIndex("IX_events_OrganizerId", "events", "OrganizerId");
            migrationBuilder.CreateIndex("IX_events_Status_StartDate", "events", new[] { "Status", "StartDate" });
            migrationBuilder.CreateIndex("IX_events_VenueId", "events", "VenueId");
            migrationBuilder.CreateIndex("IX_orders_AttendeeId", "orders", "AttendeeId");
            migrationBuilder.CreateIndex("IX_reviews_AttendeeId", "reviews", "AttendeeId");
            migrationBuilder.CreateIndex("IX_reviews_EventId_AttendeeId", "reviews", new[] { "EventId", "AttendeeId" }, unique: true);
            migrationBuilder.CreateIndex("IX_ticket_types_EventId", "ticket_types", "EventId");
            migrationBuilder.CreateIndex("IX_tickets_AttendeeId", "tickets", "AttendeeId");
            migrationBuilder.CreateIndex("IX_tickets_Code", "tickets", "Code", unique: true);
            migrationBuilder.CreateIndex("IX_tickets_OrderId", "tickets", "OrderId");
            migrationBuilder.CreateIndex("IX_tickets_TicketTypeId", "tickets", "TicketTypeId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_events_end_after_start",
                table: "events",
                sql: "\"EndDate\" > \"StartDate\"");
            migrationBuilder.AddCheckConstraint(
                name: "CK_ticket_types_quantity",
                table: "ticket_types",
                sql: "\"Quantity\" >= 0");
            migrationBuilder.AddCheckConstraint(
                name: "CK_ticket_types_sold_count",
                table: "ticket_types",
                sql: "\"SoldCount\" <= \"Quantity\"");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint("CK_events_end_after_start", "events");
            migrationBuilder.DropCheckConstraint("CK_ticket_types_quantity", "ticket_types");
            migrationBuilder.DropCheckConstraint("CK_ticket_types_sold_count", "ticket_types");

            migrationBuilder.DropIndex("IX_attendees_Email", "attendees");
            migrationBuilder.DropIndex("IX_categories_Name", "categories");
            migrationBuilder.DropIndex("IX_EventCategories_EventsId", "EventCategories");
            migrationBuilder.DropIndex("IX_events_OrganizerId", "events");
            migrationBuilder.DropIndex("IX_events_Status_StartDate", "events");
            migrationBuilder.DropIndex("IX_events_VenueId", "events");
            migrationBuilder.DropIndex("IX_orders_AttendeeId", "orders");
            migrationBuilder.DropIndex("IX_reviews_AttendeeId", "reviews");
            migrationBuilder.DropIndex("IX_reviews_EventId_AttendeeId", "reviews");
            migrationBuilder.DropIndex("IX_ticket_types_EventId", "ticket_types");
            migrationBuilder.DropIndex("IX_tickets_AttendeeId", "tickets");
            migrationBuilder.DropIndex("IX_tickets_Code", "tickets");
            migrationBuilder.DropIndex("IX_tickets_OrderId", "tickets");
            migrationBuilder.DropIndex("IX_tickets_TicketTypeId", "tickets");
        }
    }
}
