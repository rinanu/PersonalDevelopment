namespace PersonalDevelopment.Domain.Entities
{
    public class Note
    {
        public int NoteId { get; set; }
        public int UserId { get; set; }
        public string? Title { get; set; }
        public string Content { get; set; } = "";
    }
}
