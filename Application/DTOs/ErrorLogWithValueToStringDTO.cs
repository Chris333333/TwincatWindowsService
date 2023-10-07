using Domain;

namespace Application.DTOs
{
    /// <summary>
    /// DTO for returning optional value to string with ErrorLogDTO
    /// </summary>
    public class ErrorLogWithValueToStringDTO
    {
        public ErrorLogWithValueToStringDTO(ErrorLogDTO errorLogDTO, string value= null)
        {
            ErrorLogDTO = errorLogDTO;
            Value = value;
        }

        public ErrorLogDTO ErrorLogDTO { get; set; }
        public string Value { get; set; }
    }
}
