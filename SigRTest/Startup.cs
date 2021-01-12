// <copyright file="Startup.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using SigRTest;

// ReSharper disable UnusedVariable

// ReSharper disable UnusedMember.Global
[assembly: OwinStartup(typeof(Startup))]

namespace SigRTest
{
    /// <summary>
    /// Startup.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Configuration.
        /// </summary>
        /// <param name="app">
        /// App.
        /// </param>
        public void Configuration(IAppBuilder app)
        {
            app.Map(
                "/signalr",
                map =>
                {
                    // Setup the CORS middleware to run before SignalR.
                    // By default this will allow all origins. You can
                    // configure the set of origins and/or http verbs by
                    // providing a cors options with a different policy.
                    map.UseCors(CorsOptions.AllowAll);
                    var hubConfiguration = new HubConfiguration
                    {
                        EnableDetailedErrors = true,

                        // You can enable JSONP by uncommenting line below.
                        // JSONP requests are insecure but some older browsers (and some
                        // versions of IE) require JSONP to work cross domain
                        // EnableJSONP = true
                    };

                    // Run the SignalR pipeline. We're not using MapSignalR
                    // since this branch already runs under the "/signalr"
                    // path.
                    map.RunSignalR(hubConfiguration);
                });
        }
    }
}