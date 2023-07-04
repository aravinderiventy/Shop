namespace Shop.Web.Utility
{
    public class Enums
    {
        public static string CouponAPIBase { get; set; }
        public static string AuthAPIBase { get; set; }
        public static string RoleAdmin = "ADMIN";
        public static string RoleCustomer = "CUSTOMER";
        public static string TokenCookie = "JWTToken";
        public enum ApiType
        {
            GET, POST, PUT, DELETE
        }
    }
}
