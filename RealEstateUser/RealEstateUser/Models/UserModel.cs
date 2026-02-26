namespace RealEstateUser.Models
{

        public class UserModel
        {
            public int UserID { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string PhoneNo { get; set; }
            public string Role { get; set; }
        }

        



    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
