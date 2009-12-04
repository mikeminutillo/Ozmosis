using System;
using System.ComponentModel.DataAnnotations;

namespace Ozmosis
{
    public abstract class Entity<T> : IEquatable<T>, IEntity
        where T : Entity<T>
    {
        [ScaffoldColumn(false)]
        public virtual long Id { get; protected set; }

        public override string ToString()
        {
            return String.Format("{0}: {1}", GetType().Name, Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj as T);
        }

        public virtual bool Equals(T other)
        {
            if (other == null)
                return false;
            return other.Id == Id;
        }
    }
}