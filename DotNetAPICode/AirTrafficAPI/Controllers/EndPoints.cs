using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using AirTrafficAPI.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Linq.Expressions;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace AirTrafficAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EndPoints : ControllerBase
    {

        [HttpGet("GetSourceLocations")]
        public List<Routes> GetSourceLocations()
        {
            List<Routes> res = new List<Routes>();

            string filePath = "C:\\Users\\abhinraw\\Documents\\AbhinavBackup\\Code\\DataSets\\routes2.csv";

            HelperMotheods HPM = new HelperMotheods();
            // Read all lines from the CSV file
            string[] lines = HPM.ExcelLineFetcher(filePath);


            // Iterate through each line starting from the second line (index 1)
            for (int i = 1; i < 1000; i++)
            {
                string[] values = lines[i].Split(',');
                try
                {
                    res.Add(new Routes()
                    {
                        Airline = values[0],
                        AirlineID = int.Parse(values[1]),
                        SourceAirport = values[2],
                        SourceAirportID = int.Parse(values[3]),
                        DestinationApirport = values[4],
                        DestinationAirportID = int.Parse(values[5]),
                        Codeshare = values[6],
                        Stops = values[7],
                        Equipment = values[8],
                    });
                }
                catch (Exception e)
                {
                    continue;
                }
            }
            return res;
        }


        [HttpPost("PathPrediction")]
        public FlightViewData PathPrediction(PathLoc LocData)
        {

           AirspaceDivider ad=new AirspaceDivider();
           SourceDestination res = new SourceDestination();
           // Patition Map into Lat and long and then get the grid  
           //var Path =  ad.CreateWeatherGrid (new Coordinate(21.1, 81.7), new Coordinate(20.2, 85.8));
             var  Path = ad.CreateWeatherGrid(new Coordinate(19.9, 20.1), new Coordinate(22, 23));
            // Pass The Grid through Huristic function That will provide optimal path and 
            AStar aStar = new AStar();
            var sortestPath=aStar.FindPath(Path.FlightPath,Path.SourceI, Path.SourceI,Path.DestinationI,Path.DestinationJ);
            Path.Path = sortestPath;
            return Path;
        }
    }
    public class HelperMotheods
    {
        private const double BlockSizeLatitude = 0.1;
        private const double BlockSizeLongitude = 0.1;

        public string[] ExcelLineFetcher(string filePath)
        {
            return File.ReadAllLines(filePath);

        }



        //public async Task DivideAirspaceAsync(Coordinate source, Coordinate destination)
        //{
        //    double minLatitude = Math.Min(source.Latitude, destination.Latitude);
        //    double maxLatitude = Math.Max(source.Latitude, destination.Latitude);
        //    double minLongitude = Math.Min(source.Longitude, destination.Longitude);
        //    double maxLongitude = Math.Max(source.Longitude, destination.Longitude);

        //    int numBlocksLatitude = (int)Math.Ceiling((maxLatitude - minLatitude) / BlockSizeLatitude);
        //    int numBlocksLongitude = (int)Math.Ceiling((maxLongitude - minLongitude) / BlockSizeLongitude);
        //    string ApiKey = "dbc40c37899e230dfbff4a263823d174";
        //    using (var client = new HttpClient())
        //    {
        //        for (int latIndex = 0; latIndex < numBlocksLatitude; latIndex++)
        //        {
        //            for (int longIndex = 0; longIndex < numBlocksLongitude; longIndex++)
        //            {
        //                double blockMinLatitude = minLatitude + latIndex * BlockSizeLatitude;
        //                double blockMaxLatitude = Math.Min(blockMinLatitude + BlockSizeLatitude, maxLatitude);
        //                double blockMinLongitude = minLongitude + longIndex * BlockSizeLongitude;
        //                double blockMaxLongitude = Math.Min(blockMinLongitude + BlockSizeLongitude, maxLongitude);

        //                // Construct API request URL for weather data
        //                //string apiUrl = $"https://api.openweathermap.org/data/2.5/weather?" +
        //                //                $"lat={blockMinLatitude}&lon={blockMinLongitude}&appid={ApiKey}";

        //                // Make API request
        //                //HttpResponseMessage response = await client.GetAsync(apiUrl);
        //                //if (response.IsSuccessStatusCode)
        //                //{
        //                //    // Read response content
        //                //    string weatherData = await response.Content.ReadAsStringAsync();
        //                //    // Process weather data as needed
        //                //    Console.WriteLine($"Weather data for block [{latIndex}, {longIndex}]: {weatherData}");
        //                //}
        //                //else
        //                //{
        //                //    Console.WriteLine($"Failed to retrieve weather data for block [{latIndex}, {longIndex}]");
        //                //}
        //            }
        //        }
        //    }
        //}
    }
}
