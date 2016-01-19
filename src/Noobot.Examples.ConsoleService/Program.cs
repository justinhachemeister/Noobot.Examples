﻿using System;
using System.IO;
using System.Reflection;
using Noobot.Core.Configuration;
using Noobot.Examples.ConsoleService.Configuration;
using Noobot.Examples.ConsoleService.Logging;
using Topshelf;

namespace Noobot.Examples.ConsoleService
{
    public class Program
    {
        private static readonly IConfigReader ConfigReader = new JsonConfigReader();
        private static readonly ILogger Logger = new ConsoleLogger(ConfigReader);

        public static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            Console.WriteLine($"Noobot.Core assembly version: {Assembly.GetAssembly(typeof(Core.INoobotCore)).GetName().Version}");

            HostFactory.Run(x =>
            {
                x.Service<NoobotHost>(s =>
                {
                    s.ConstructUsing(name => new NoobotHost(ConfigReader));

                    s.WhenStarted(n =>
                    {
                        Logger.Grapple();
                        n.Start();
                    });

                    s.WhenStopped(n => n.Stop());
                });

                x.RunAsNetworkService();
                x.SetDisplayName("Noobot");
                x.SetServiceName("Noobot");
                x.SetDescription("An extensible Slackbot built in C#");
            });

            Logger?.Dispose();
        }
    }
}
