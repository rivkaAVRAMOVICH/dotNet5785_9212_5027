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
    none,
    fixing,
    cooking,
    babysitting,
    cleaning,
    shopping
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
    YEAR
}
