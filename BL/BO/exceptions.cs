namespace BO;

[Serializable]
public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }
    public BlDoesNotExistException(string message, Exception innerException)
                : base(message, innerException) { }
}

[Serializable]
public class BlAlreadyExistException : Exception
{
    public BlAlreadyExistException(string? message) : base(message) { }
    public BlAlreadyExistException(string message, Exception innerException)
                : base(message, innerException) { }
}

[Serializable]
public class BlDeletionImpossible : Exception
{
    public BlDeletionImpossible(string? message) : base(message) { }
    public BlDeletionImpossible(string message, Exception innerException)
                : base(message, innerException) { }
}

[Serializable]
public class BlInvalidAssignmentException : Exception
{
    public BlInvalidAssignmentException(string message) : base(message) { }
    public BlInvalidAssignmentException(string message, Exception innerException) : base(message, innerException) { }
}

[Serializable]
public class BlUnauthorizedActionException : Exception
{
    public BlUnauthorizedActionException(string message) : base(message) { }
    public BlUnauthorizedActionException(string message, Exception innerException) : base(message, innerException) { }
}

[Serializable]
public class BlAlreadyCompletedException : Exception
{
    public BlAlreadyCompletedException(string message) : base(message) { }
    public BlAlreadyCompletedException(string message, Exception innerException) : base(message, innerException) { }
}

[Serializable]
public class BlValidationException : Exception
{
    public BlValidationException(string message) : base(message) { }
    public BlValidationException(string message, Exception innerException) : base(message, innerException) { }
}

[Serializable]
public class BlUnexpectedSystemException : Exception
{
    public BlUnexpectedSystemException(string message) : base(message) { }
    public BlUnexpectedSystemException(string message, Exception innerException) : base(message, innerException) { }
}

[Serializable]
public class BlFormatException : Exception
{
    public BlFormatException(string? message) : base(message) { }
    public BlFormatException(string message, Exception innerException)
                : base(message, innerException) { }
}

[Serializable]
public class BlNullPropertyException : Exception
{
    public BlNullPropertyException(string? message) : base(message) { }
}


[Serializable]
public class ArgumentException : Exception
{
    public ArgumentException(string? message) : base(message) { }
}

[Serializable]
public class UnauthorizedAccessException : Exception
{
    public UnauthorizedAccessException(string? message) : base(message) { }
}

[Serializable]
public class InvalidException : Exception
{
    public InvalidException(string? message) : base(message) { }
}





