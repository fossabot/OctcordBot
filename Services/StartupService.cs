﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace OctcordBot.Services {
    public class StartupService {

        private readonly IServiceProvider    _provider;
        private readonly DiscordSocketClient _discord;
        private readonly CommandService      _commands;
        private readonly IConfigurationRoot  _config;

        // DiscordSocketClient, CommandService, and IConfigurationRoot are injected automatically from the IServiceProvider
        public StartupService(
            IServiceProvider    provider,
            DiscordSocketClient discord,
            CommandService      commands,
            IConfigurationRoot  config) {
            _provider = provider;
            _config   = config;
            _discord  = discord;
            _commands = commands;
        }

        private void PromptFor(string requestedItem, string configKey) {
            if (!string.IsNullOrWhiteSpace(_config[configKey])) return;

            Console.Write($"{requestedItem}: ");
            _config[configKey] = Console.ReadLine();
        }

        public async Task StartAsync() {
            PromptFor("Discord Bot Token",        "discord_token");
            PromptFor("Command Prefix",           "command_prefix");
            PromptFor("Discord Admin Channel ID", "bot_channel");
            PromptFor("GitHub Token",             "github_token");
            PromptFor("GitHub Owner",             "github_owner");
            PromptFor("GitHub Repo",              "github_repo");

            Console.WriteLine("You can use the command below in the admin channel to continue setup in Discord.");
            Console.WriteLine();
            Console.WriteLine($"    {_config["command_prefix"]}SetupHelp");
            Console.WriteLine();

            await _discord.LoginAsync(TokenType.Bot, _config["discord_token"]);
            await _discord.StartAsync();

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        }

    }
}
