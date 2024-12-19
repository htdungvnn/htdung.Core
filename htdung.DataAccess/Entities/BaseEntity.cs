namespace htdung.DataAccess.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateTime { get; set; }
        public bool Status { get; set; }
        public Guid UserCreated { get; set; }
        public Guid UserEdited { get; set; }
    }
}
