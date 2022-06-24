namespace Hyland.Rocket.RPA.ApiHelper.Messages
{
    public class LoginBody
    {
        public string identifier { get; set; }
        public string password { get; set; }
        public bool remember { get; set; }
        public string redirectUrl { get; set; }
    }
}
