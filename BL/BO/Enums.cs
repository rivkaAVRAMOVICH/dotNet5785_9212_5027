namespace BO;

public enum Role
{
    manager,
    volunteer,
    None
}

public enum TypeOfDistance
{
    Air,
    Walking,
    Driving,
    None
}

public enum CallType
{
    fixing,
    cooking,
    babysitting,
    cleaning,
    shopping,
    none
}

public enum Status
{
    open,
    inProgress,
    closed,
    expired,
    openAtRisk,
    inProgressAtRisk
}

public enum FinishType
{
    takenCareOf,
    volunteerCanceled,
    managerCanceledAssignment,
    expired
}

public enum TimeUnit
{
    MINUTE,
    HOUR,
    DAY,
    MONTH,
    SECOND,
    YEAR
}

public enum AssignmentSortField
{
    Id,
    VolunteerId,
    CallId,
    Status,
    Distance,
    CallType,
    FinishType
}
public enum CallSortField
{
    Id,
    CallType,
    FinishType
}

public enum  VolunteerSortField
{   
    Id,
    Name,
    Phone,
    Distance,
    Status,
    CallType,
    FinishType
}