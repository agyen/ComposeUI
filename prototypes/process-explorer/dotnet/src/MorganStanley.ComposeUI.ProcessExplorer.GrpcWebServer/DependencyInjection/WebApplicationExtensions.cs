using MorganStanley.ComposeUI.ProcessExplorer.GrpcWebServer.Server.Infrastructure.Grpc;

namespace Microsoft.Extensions.DependencyInjection;

public static class WebApplicationExtensions
{
    public static WebApplication AddGrpcMessageService(this WebApplication webApplication)
    {
        webApplication.UseGrpcWeb();
        webApplication.UseCors();
        webApplication.MapGrpcService<ProcessExplorerMessageHandlerService>().EnableGrpcWeb().RequireCors("AllowAll");
        webApplication.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

        return webApplication;
    }
}
