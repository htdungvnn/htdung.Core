using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace htdung.DataAccess.Entities
{
    public class BaseActionLog<T>(T entity, ActionType actionType, Guid userHandle)
        where T : BaseEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public Guid Id { get; set; } = entity.Id;

        public ActionType ActionType { get; set; } = actionType;
        public T Entity { get; set; } = entity;
        public Guid UserHandle { get; set; } = userHandle;
        public DateTime ActionDate { get; set; } = DateTime.UtcNow;
    }

    public enum ActionType
    {
        Create,
        Update
    }
}
