﻿namespace Framework.Domain.Entitities
{
    using System;

    /// <summary>
    /// The nh entity.
    /// </summary>
    [Serializable]
    public abstract class EntityBase
    {
        /// <summary>
        /// Gets a value indicating whether is new.
        /// </summary>
        public virtual bool IsNew
        {
            get { return this.Id == Guid.Empty; }
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public virtual Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the create date.
        /// </summary>
        public virtual DateTime CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the update date.
        /// </summary>
        public virtual DateTime? UpdateDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether deleted.
        /// </summary>
        public virtual bool Deleted { get; set; }
    }
}
