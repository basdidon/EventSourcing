namespace Api.Const
{
    public static class Role
    {
        public const string Admin = "Admin";
        public const string Teller = "Teller";
        public const string Customer = "Customer";

        public static string[] All => [Admin, Teller, Customer];
    }
    /*
    public enum Roles
    {
        Admin,
        Teller,
        Customer
    }*/
}
