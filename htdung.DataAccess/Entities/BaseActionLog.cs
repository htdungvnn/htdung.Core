using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace htdung.DataAccess.Entities
{
    public class BaseActionLog<T> where T : BaseEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public Guid Id { get; set; }
        public BaseActionLog(T entity, ActionType actionType, Guid userHandle)
        {
            Entity = entity;
            ActionType = actionType;
            UserHandle = userHandle;
            Id = entity.Id;
        }

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
