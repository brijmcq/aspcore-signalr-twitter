using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tweetinvi;
using Tweetinvi.Models;

namespace tweetToMap
{
    public class Startup
    {

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            TelemetryConfiguration.Active.DisableTelemetry = true;
            services.AddTransient<Chat,Chat>();
            services.AddSingleton<IHostedService, TwitterFilteredStream>();
            Auth.SetUserCredentials("deSIaynruoSerhGyiWP0IqBCW", "GYSsEUYhGn1Cp3xJVVij2oiHjnpI48eq7bsAv4YZ8fCcDFM6hY",
                "2195279515-NP2DA1dH3MurcUil1KKo8V1N2ERlQsC8A1zAnfh", "7C5CeETWEEEynmDYgmV9Z5Z51miYNu1ToOGCOlZQSoary");


            var authenticatedUser = User.GetAuthenticatedUser();
            services.AddCors(o => o.AddPolicy("appCors", b =>
           {
               b
               .AllowAnyMethod()
               .AllowAnyHeader()
               .WithOrigins("http://localhost:4200");
           }));
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            IHubContext<Chat> chatHub)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("appCors");
            app.UseSignalR(r =>
            {
                r.MapHub<Chat>("chat");
            });

        //    MyTwitterStream(chatHub);

        }

        private static void MyTwitterStream(IHubContext<Chat> chatHub)
        {
            var stream = Stream.CreateFilteredStream();

            //Coordinates of manila 14.599512, 120.984222
            stream.AddLocation(new Coordinates(14, 119.5), new Coordinates(15, 121.5));
            stream.AddTrack("manila");
            stream.MatchingTweetReceived += (sender, args) =>
            {
                Thread.Sleep(1000);
                Console.WriteLine(args.Tweet);
                chatHub.Clients.All.InvokeAsync("sendToAll", "the geolocation is", $"{args.Tweet.Coordinates}");

            };

            stream.StartStreamMatchingAllConditions();
        }
    }
}
