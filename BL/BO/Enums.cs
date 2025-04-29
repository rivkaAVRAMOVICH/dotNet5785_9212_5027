namespace BO;

public enum Role
{
    manager,
    volunteer
}

public enum TypeOfDistance
{
    Air,
    Walking,
    Driving
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
public enum CallSortField
{
    Id,
    CallType,
    FinishType
}