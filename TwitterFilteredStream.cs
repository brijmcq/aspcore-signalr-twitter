using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;

namespace tweetToMap
{
    public class TwitterFilteredStream : BackgroundService
    {
        private readonly IHubContext<Chat> hubContext;

        public TwitterFilteredStream(IHubContext<Chat> hubContext)
        {
            this.hubContext = hubContext;
        }
        public static void StartStream()
        {

            var stream = Stream.CreateFilteredStream();

            //Coordinates of manila 14.599512, 120.984222
            stream.AddLocation(new Coordinates(14, 119.5), new Coordinates(15, 121.5));
            stream.AddTrack("manila");
            stream.MatchingTweetReceived += (sender, args) =>
            {
                Thread.Sleep(1000);
                Console.WriteLine(args.Tweet);
                //var myHub = new Chat();
                //myHub.SendToAll("the geolocation is", $"{args.Tweet.Coordinates}");
            };
            stream.StartStreamMatchingAnyConditionAsync();

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var stream = Stream.CreateFilteredStream();

            //Coordinates of manila 14.599512, 120.984222


            //stream.AddLocation(new Coordinates(14, 119.5), new Coordinates(15, 121.5));

            // this one is more accurate
            stream.AddLocation(new Coordinates(14.734378286697154, 120.9591293334961
), new Coordinates(14.419717892223632, 121.04255676269531));

            stream.AddTrack("manila");
            stream.MatchingTweetReceived += (sender, args) =>
            {
                //Delay retrieving of tweets to prevent spam!
                //Thread.Sleep(1000);
                Console.WriteLine(args.Tweet);
                //send only tweets with coordinates
                if (args.Tweet.Coordinates != null)
                {
                    hubContext.Clients.All.InvokeAsync("sendToAll", new
                    {
                        lat = args.Tweet.Coordinates.Latitude,
                        lng = args.Tweet.Coordinates.Longitude,
                        title = args.Tweet.Text,
                        url = args.Tweet.Url
                    });
                }
                    
                //hubContext.Clients.All.InvokeAsync("sendToAll", "the geolocation is", $"{args.Tweet.Coordinates.Latitude}, {args.Tweet.Coordinates.Longitude}");

            };
            stream.StartStreamMatchingAnyCondition();



            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
            //stop stream
            stream.StopStream();



        }
    }
}
