namespace Domain
{
    public class TwincatLayoutsDictionary
    {
        public TwincatLayoutsDictionary(string mainLayoutName, TwincatDataTypes twincatDataType )
        {
            MainLayoutName = mainLayoutName;
            TwincatDataType = twincatDataType;
        }

        public string MainLayoutName { get; set; }
        public TwincatDataTypes TwincatDataType { get; set; }
    }
}
