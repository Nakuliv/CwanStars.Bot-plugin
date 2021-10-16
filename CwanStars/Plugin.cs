using System;
using System.Collections.Generic;
using System.Text;
using Sapiox.API;
using Sapiox.Events.EventArgs;
using SimpleTcp;
using UnityEngine;
using PlayerEvent = Sapiox.Events.Handlers.Player;

namespace CwanStars
{
    [PluginInfo(
        Author = "Naku",
        Name = "CwanStars.PluginPack",
        Description = "Example plugin",
        Version = "1.0.0"
        )]
    
    public class Plugin : Sapiox.API.Plugin
    {
        private static SimpleTcpServer server = new SimpleTcpServer("127.0.0.1:9000");
        public override IConfig config { get; set; } = new Config();
        public static string ClientIpPort;
        public List<string> PlayerList = new List<string>();
        
        
        public string listagraczy()
        {
            return string.Join("\n", PlayerList);
        }

        public override void Load()
        {
            base.Load();
            PlayerEvent.Join += OnPlayerJoin;
            PlayerEvent.Leave += OnPlayerLeave;
            PlayerEvent.Ban += OnPlayerBan;
            PlayerEvent.Kick += OnPlayerKick;
            Sapiox.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
            server.Events.ClientConnected += ClientConnected;
            server.Events.ClientDisconnected += ClientDisconnected;
            server.Events.DataReceived += DataReceived;
            
            server.Start();
        }

        public void OnWaitingForPlayers()
        {
            Log.Info("CwanStars: Oczekiwanie na graczy...");
            PlayerList.Clear();
        }
        public void OnPlayerBan(PlayerBanEventArgs ev)
        {
            Log.Info($"Player {ev.Target.NickName} gets banned for {ev.Reason} {ev.Duration}");
        }

        public void OnPlayerKick(PlayerKickEventArgs ev)
        {
            Log.Info($"Player {ev.Target.NickName} gets kicked for {ev.Reason}");
        }
        public void OnPlayerLeave(PlayerLeaveEventArgs ev)
        {
            string message = "";
            PlayerList.Remove(ev.Player.NickName);
            message += listagraczy();
            
            if (string.IsNullOrEmpty(message))
                message = "No players online";
            
            server.Send(ClientIpPort, $"N-aCj)qa.&q<hzt! Players online: ****{PlayerList.Count}/{CustomNetworkManager.slots}****\nPlayers list:\n" + "****" + message + "\n" + "****");
            
            Log.Info($"Player {ev.Player.NickName} has left the server!");
            
            Server.SendDiscordWebhook(
                token: "https://canary.discord.com/api/webhooks/878323051849216091/CWxCfYLL1LXmnazKvwsk6AyGZFOBifoR4dtt8e8hiXyeM6yBa9yMVvWXHy4rtq-Cv-0u",
                username: "CwanStars Logs",
                content:"** **",
                description: $"**SteamID:** {ev.Player.UserId}\n**Rank:** {ev.Player.Hub.serverRoles.Network_myText}",
                embed: true,
                embedTitle: $"Player {ev.Player.NickName} has left the server!",
                thumbnailUrl: "https://cdn.discordapp.com/emojis/643818277230870528.png?v=1",
                embedColor: 15158332
            );
        }
        public void OnPlayerJoin(PlayerJoinEventArgs ev)
        {
            string message = "";
            PlayerList.Add(ev.NickName);
            message += listagraczy();
            
            if (string.IsNullOrEmpty(message))
                message = "No players online";
            
            server.Send(ClientIpPort, $"N-aCj)qa.&q<hzt! Players online: ****{PlayerList.Count}/{CustomNetworkManager.slots}****\nPlayers list:\n" + "****" + message + "\n" + "****");
            
            Log.Info($"Player {ev.NickName} has joined the server!");
            
            ev.Player.Broadcast(10, "Welcome to <color=#800080>Night</color><color=yellow>Raccoons</color>!");
            
            Server.SendDiscordWebhook(
                token: "https://canary.discord.com/api/webhooks/878323051849216091/CWxCfYLL1LXmnazKvwsk6AyGZFOBifoR4dtt8e8hiXyeM6yBa9yMVvWXHy4rtq-Cv-0u",
                username: "CwanStars Logs",
                content:"** **",
                description: $"**SteamID:** {ev.Player.UserId}\n**Rank:** {ev.Player.Hub.serverRoles.Network_myText}",
                embed: true,
                embedTitle: $"Player {ev.NickName} has joined the server!",
                thumbnailUrl: "https://cdn.discordapp.com/emojis/643818277176606720.png?v=1",
                embedColor: 3066993
            );
        }
        
        static void ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            Log.Info("*** Server connected");
            ClientIpPort = e.IpPort;
        }

        static void ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            Log.Info("*** Server disconnected");
        }

        public static void DataReceived(object sender, DataReceivedEventArgs e)
        {
            ClientIpPort = e.IpPort;
            //GameCore.Console.singleton.TypeCommand(Encoding.UTF8.GetString(e.Data));
            Log.Info("[" + e.IpPort + "] " + Encoding.UTF8.GetString(e.Data));
        }
    }
}