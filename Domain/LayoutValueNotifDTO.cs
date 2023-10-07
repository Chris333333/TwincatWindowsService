namespace Domain
{
    public class LayoutValueNotifDTO
    {
        public LayoutValueNotifDTO(string layoutName, string changedValueNotif)
        {
            LayoutName = layoutName;
            ChangedValueNotif = changedValueNotif;
        }

        public string LayoutName { get; set; }
        public string ChangedValueNotif { get; set; }
    }
}
