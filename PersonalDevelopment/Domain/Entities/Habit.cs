namespace PersonalDevelopment.Domain.Entities
{
    public class Habit
    {
        public int HabitId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public string Frequency { get; set; } = "daily";
        public bool IsActive { get; set; }
    }
}
