namespace MSTART_Hiring_Task.Models.Currencies
{
    public struct Currencies
    {
        public const string USD = "USD";
        public const string EUR = "EUR";
        public const string GBP = "GBP";
        public const string JOR = "Dinar";


        public static IEnumerable<string> GetAll()
        {
            return new List<string> { USD, EUR, GBP, JOR };
        }
    }
}
