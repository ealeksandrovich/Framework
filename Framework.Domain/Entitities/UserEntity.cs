namespace Framework.Domain.Entitities
{
    using System;

    public class UserEntity: EntityBase
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}