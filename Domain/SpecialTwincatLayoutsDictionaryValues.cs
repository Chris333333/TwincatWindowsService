namespace Domain
{
    public class SpecialTwincatLayoutsDictionaryValues : TwincatLayoutsDictionaryValues
    {
        public SpecialTwincatLayoutsDictionaryValues(string mainLayoutName, int partIndex, TwincatDataTypes twincatDataTypes, string value = null ) : base(mainLayoutName,twincatDataTypes, value)
        { 
            PartIndex = partIndex;
        }

        public int PartIndex { get; set; }


    }
}
