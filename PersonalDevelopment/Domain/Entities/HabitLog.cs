namespace PersonalDevelopment.Domain.Entities
{
    public class HabitLog
    {
        public int HabitId { get; set; }
        public DateTime LogDate { get; set; }
        public bool Status { get; set; }
        public string? Note { get; set; }
    }
}
