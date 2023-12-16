namespace Api.Gateway.Services;

public interface ICommandService
{
     Task<Guid?> PlaceOrder();
}