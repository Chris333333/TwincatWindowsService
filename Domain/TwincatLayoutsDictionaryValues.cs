namespace Domain
{
    public class TwincatLayoutsDictionaryValues : TwincatLayoutsDictionary
    {
        public TwincatLayoutsDictionaryValues(string layoutname, TwincatDataTypes twincatDataTypes, string value = null, int stringLength = 0) : base(layoutname, twincatDataTypes)
        {
            Value = value;
            StringLength = stringLength;
        }
        public string Value { get; set; }
        public int StringLength { get; set; }
    }
}
