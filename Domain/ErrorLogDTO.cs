namespace Domain
{
    public class ErrorLogDTO
    {
        public ErrorLogDTO(bool isError, string message = null)
        {
            IsError = isError;
            Message = message;
        }

        public bool IsError { get; set; }
        public string Message { get; set; }
    }
}
