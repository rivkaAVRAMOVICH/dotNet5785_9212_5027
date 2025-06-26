
namespace BO
{
    [Serializable]
    internal class BLTemporaryNotAvailableException : Exception
    {
        public BLTemporaryNotAvailableException()
        {
        }

        public BLTemporaryNotAvailableException(string? message) : base(message)
        {
        }

        public BLTemporaryNotAvailableException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}