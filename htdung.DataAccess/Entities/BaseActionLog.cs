using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace htdung.DataAccess.Entities
{
    public class BaseActionLog<T> where T : BaseEntity
    {
        public ActionType ActionType { get; set; }
        public T Entity { get; set; }
        public Guid UserHandle { get; set; }
        public DateTime ActionDate { get; set; } = DateTime.UtcNow;
    }

    public enum ActionType
    {
        Create,
        Update,
        Delete
    }
}
