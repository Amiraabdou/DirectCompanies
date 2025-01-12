namespace DirectCompanies.Helper
{
    public static class DecimalHelper
    {
        private static decimal _lasID;

        static DecimalHelper()
        {
            _lasID = default(decimal);
        }
        public static decimal NewID()
        {
            string text = BitConverter.ToUInt64(Guid.NewGuid().ToByteArray(), 8).ToString();
            long ticks = DateTime.Now.Ticks;
            int length = 8;
            if (text.Length < 8)
            {
                length = text.Length;
            }

            decimal num = decimal.Parse($"{ticks}.{text.Substring(0, length)}");
            if (num == _lasID)
            {
                return NewID();
            }

            _lasID = num;
            return num;
        }

    }
}
