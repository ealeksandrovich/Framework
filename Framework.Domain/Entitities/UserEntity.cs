namespace Framework.Domain.Entitities
{
    public class UserEntity: EntityBase
    {
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}