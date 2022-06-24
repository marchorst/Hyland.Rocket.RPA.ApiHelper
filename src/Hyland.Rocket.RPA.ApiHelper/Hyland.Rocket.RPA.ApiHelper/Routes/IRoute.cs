namespace Hyland.Rocket.RPA.ApiHelper.Routes
{
    public interface IRoute
    {
        string BearerToken { get; set; }
        string DomainWithProtocol { get; set; }
    }
}
