namespace BarberFlow.Domain.Enums;

public enum AppointmentRequestRejectionReason
{
    ScheduleConflict = 1,
    OutsideWorkingHours = 2,
    EmployeeInactive = 3,
    ServiceInactive = 4,
    EmployeeNotQualified = 5,
    InvalidStartTime = 6,
    AppointmentNotChangeable = 7,
    Unauthorized = 8
}
