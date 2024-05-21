using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Security.Cryptography;

namespace AirTrafficAPI.Models
{
    public class Routes
    {
        public string Airline { get; set; }
        public int AirlineID { get; set; }
        public string SourceAirport { get; set; }
        public int SourceAirportID { get; set; }
        public string DestinationApirport { get; set; }
        public int DestinationAirportID { get; set; }
        public string Codeshare { get; set; }
        public string Stops { get; set; }
        public string Equipment { get; set; }
       

    }
    public class SourceDestination
    {
        public AirLocation Source {  get; set; }
        public AirLocation Destination { get; set; }
    }
    public class PathLoc
    {
        public SourceDestination EndPointsLoc { get; set; }
        public AirLocation CurrentLoc { get; set; }
    }
    public class AirLocation
    {
        public int Lat;
        public int Long;
        public int Alt;
    }
    public class Coordinate
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Coordinate(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
    public class FlightViewData
    {
        public Coordinate Source {  get; set; }
        public int SourceI { get; set; }
        public int SourceJ { get; set; }
        public int DestinationI { get; set; }
        public int DestinationJ { get; set; }
        public Coordinate Destination { get; set; }
        public List<List<WeatherBlock>> FlightPath { get; set; }
        public List<FlightPath> Path { get; set; }
    }
    public class WeatherBlock
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string WeatherCondition { get; set; }
        public double HeuristicValue { get; set; }
        public bool isPath = false;

        public WeatherBlock(double latitude, double longitude, string weatherCondition, double heuristicValue)
        {
            Latitude = latitude;
            Longitude = longitude;
            WeatherCondition = weatherCondition;
            HeuristicValue = heuristicValue;
        }
    }
    public class FlightPath
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int i { get; set; }
        public int j { get; set; }

        public FlightPath(double latitude, double longitude, int ii, int yy)
        {
            Latitude = latitude;
            Longitude = longitude;
            i = ii;
            j = yy;
           
        }
    }
    public class AirspaceDivider
    {
        private const double BlockSizeLatitude = 0.1;
        private const double BlockSizeLongitude = 0.1;

        public FlightViewData CreateWeatherGrid(Coordinate source, Coordinate destination)
        {
            double padding = 1;
            double minLatitude = Math.Min(source.Latitude, destination.Latitude) - padding;
            double maxLatitude = Math.Max(source.Latitude, destination.Latitude) + padding;
            double minLongitude = Math.Min(source.Longitude, destination.Longitude) - padding;
            double maxLongitude = Math.Max(source.Longitude, destination.Longitude) + padding;


            int numBlocksLatitude = (int)Math.Ceiling((maxLatitude - minLatitude) / BlockSizeLatitude);
            int numBlocksLongitude = (int)Math.Ceiling((maxLongitude - minLongitude) / BlockSizeLongitude);

            List<List<WeatherBlock>> weatherGrid = new List<List<WeatherBlock>>();
            FlightViewData res = new FlightViewData();
            int sourceIndexI = -1, sourceIndexJ = -1;
            int destinationIndexI = -1, destinationIndexJ = -1;

            Random random = new Random();

            for (int latIndex = 0; latIndex < numBlocksLatitude; latIndex++)
            {
                List<WeatherBlock> row = new List<WeatherBlock>();
                

                for (int longIndex = 0; longIndex < numBlocksLongitude; longIndex++)
                {
                    double blockMinLatitude = minLatitude + latIndex * BlockSizeLatitude;
                    double blockMaxLatitude = Math.Min(blockMinLatitude + BlockSizeLatitude, maxLatitude);
                    double blockMinLongitude = minLongitude + longIndex * BlockSizeLongitude;
                    double blockMaxLongitude = Math.Min(blockMinLongitude + BlockSizeLongitude, maxLongitude);

                    string[] possibleConditions = { "Clear", "Partly Cloudy", "Cloudy", "Rainy", "ThunderStorm" };
                    string randomCondition = possibleConditions[random.Next(possibleConditions.Length)];

                    double randomHeuristic = random.NextDouble();
                    // Fetch Huristics value here:-
                    double weatherHuristics = CalculateWeatherHeuristic(randomCondition);
                    double distance = CalculateDistance(new WeatherBlock(blockMaxLatitude - blockMinLatitude, blockMaxLongitude - blockMinLongitude,"",0.1), destination);
               


                    WeatherBlock block = new WeatherBlock(blockMinLatitude, blockMinLongitude, randomCondition, weatherHuristics* distance);
                    row.Add(block);
                    // Check if this block contains source or destination coordinate
                    if (source.Latitude >= blockMinLatitude && source.Latitude < blockMaxLatitude &&
                        source.Longitude >= blockMinLongitude && source.Longitude < blockMaxLongitude)
                    {
                        sourceIndexI = latIndex;
                        sourceIndexJ = longIndex;
                    }
                    if (destination.Latitude >= blockMinLatitude && destination.Latitude < blockMaxLatitude &&
                        destination.Longitude >= blockMinLongitude && destination.Longitude < blockMaxLongitude)
                    {
                        destinationIndexI = latIndex;
                        destinationIndexJ = longIndex;
                    }
                }

                weatherGrid.Add(row);
            }
            res.DestinationI = destinationIndexI;
            res.DestinationJ = destinationIndexJ;
            res.SourceI= sourceIndexI;
            res.SourceJ = sourceIndexJ;
            res.FlightPath = weatherGrid;
            return res;
        }
        public WeatherBlock[,] ConvertToAirspaceOutput(List<List<WeatherBlock>> airspaceOutput)
        {
            if (airspaceOutput == null || airspaceOutput.Count == 0 || airspaceOutput[0].Count == 0)
                return null; // or throw an exception, depending on your requirements

            int numRows = airspaceOutput.Count;
            int numCols = airspaceOutput[0].Count;

            var grid = new WeatherBlock[numRows, numCols];

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    grid[i, j] = airspaceOutput[i][j];
                }
            }

            return  grid ;
        }
        private double CalculateWeatherHeuristic(string condition)
        {
            // Calculate heuristic value based on weather conditions
            // Adjust as needed based on your requirements
            switch (condition)
            {
                case "Clear":
                    return 10;
                case "Partly Cloudy":
                    return 10;
                case "Cloudy":
                    return 20;
                case "Rainy":
                    return 30;
                case "Thunderstorm":
                    return 100;
                default:
                    return 100; // Default value for unknown conditions
            }
        }
        private double CalculateDistance(WeatherBlock block, Coordinate destination)
        {
            // Calculate Euclidean distance between block and destination
            return (Math.Pow(block.Latitude - destination.Latitude, 2) + Math.Pow(block.Longitude - destination.Longitude, 2));
        }
    }
}
