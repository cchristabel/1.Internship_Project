using System.Collections.Generic;


namespace SecureMVCApp.Models  
{
    public class UserConfig
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public List<UserClaim> Claims { get; set; }
        }

        public class UserClaim
        {
            public string Type { get; set; }
            public string Value { get; set; }
        }

        public class UsersConfig
        {
            public List<UserConfig> Users { get; set; }
        }

}
