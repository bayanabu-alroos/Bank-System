namespace MSTART_Hiring_Task.Models
{
    public class SharedProp
    {
        public DateTime Server_DateTime { get; set; } = DateTime.Now;
        public DateTime DateTime_UTC { get; set; } = DateTime.Now;
        public DateTime Update_DateTime_UTC { get; set; } = DateTime.Now;
    }
}
