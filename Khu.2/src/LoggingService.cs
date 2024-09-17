using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace OpenRoadHelper
{
    public class LoggingService
    {
        public LoggingService(DiscordSocketClient client, CommandService command)
        {
            client.Log += LogAsync;
            command.Log += LogAsync;
        }

        private Task LogAsync(LogMessage message)
        {
            if (message.Exception is CommandException cmdException)
            {
                Logger.Error(
                    $"[Command/{message.Severity}] {cmdException.Command.Aliases[0]}"
                        + $" failed to execute in {cmdException.Context.Channel}."
                );
                Logger.Error(cmdException.ToString());
            }
            else
                Logger.Error($"[General/{message.Severity}] {message}");

            return Task.CompletedTask;
        }
    }
}
