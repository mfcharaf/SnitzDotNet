using System;

namespace Snitz.Entities
{
    public class RoleInfo
    {
        public int Id { get; set; }
        public Guid ApplicationId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public string LoweredRolename { get; set; }
    }

    public class UsersInRoleInfo
    {
        public int MemberId { get; set; }
        public int RoleId { get; set; }
    }
}
