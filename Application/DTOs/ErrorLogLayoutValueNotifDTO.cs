using Domain;

namespace Application.DTOs
{
    /// <summary>
    /// Special DTO witch connects ErrorDTO and LayoutValueNotifDTO
    /// </summary>
    public class ErrorLogLayoutValueNotifDTO
    {
        public ErrorLogLayoutValueNotifDTO(ErrorLogDTO errorLogDTO, LayoutValueNotifDTO layoutValueNotifDTO = null)
        {
            ErrorLogDTO = errorLogDTO;
            LayoutValueNotifDTO = layoutValueNotifDTO;
        }

        public ErrorLogDTO ErrorLogDTO { get; set; }
        public LayoutValueNotifDTO LayoutValueNotifDTO { get; set;}
    }
}
