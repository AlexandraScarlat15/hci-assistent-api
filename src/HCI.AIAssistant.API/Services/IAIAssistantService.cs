namespace HCI.AIAssistant.API.Services;

public interface IAIAssistantService
{
    public Task<string> SendMessageAndGetResponseAsync(string message);
}