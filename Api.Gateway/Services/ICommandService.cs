namespace Api.Gateway.Services;

public interface ICommandService
{
     Task<string> PlaceOrder();
}