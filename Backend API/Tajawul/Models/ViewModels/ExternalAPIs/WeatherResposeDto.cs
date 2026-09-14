namespace Tajawul.Models.ViewModels.Weather
{
    public class WeatherResposeDto
    {
        //public int queryCost { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string Address { get; set; }
        public string ResolvedAddress { get; set; }
        public string Timezone { get; set; }
        public double TzOffset { get; set; }
        public string Description { get; set; }
        public Currentconditions CurrentConditions { get; set; }
        public object[] Alerts { get; set; }
        public Day[] Days { get; set; }
        //public Stations stations { get; set; }
    }

    //public class Stations
    //{
    //    public HECA HECA { get; set; }
    //}

    //public class HECA
    //{
    //    public int distance { get; set; }
    //    public float latitude { get; set; }
    //    public float longitude { get; set; }
    //    public int useCount { get; set; }
    //    public string id { get; set; }
    //    public string name { get; set; }
    //    public int quality { get; set; }
    //    public int contribution { get; set; }
    //}

    public class Currentconditions
    {
        public string Datetime { get; set; }
        public int DatetimeEpoch { get; set; }
        public string Conditions { get; set; }
        public string Icon { get; set; }
        public float Temp { get; set; }
        public float Feelslike { get; set; }
        public float Humidity { get; set; }
        public float Dew { get; set; }
        public object Precip { get; set; }
        public double Precipprob { get; set; }
        public double Snow { get; set; }
        public double Snowdepth { get; set; }
        public object Preciptype { get; set; }
        public object Windgust { get; set; }
        public float Windspeed { get; set; }
        public double Winddir { get; set; }
        public double Pressure { get; set; }
        public float Visibility { get; set; }
        public double Cloudcover { get; set; }
        public double Solarradiation { get; set; }
        public double Solarenergy { get; set; }
        public double Uvindex { get; set; }
        //public string[] stations { get; set; }
        //public string source { get; set; }
        public string Sunrise { get; set; }
        public int SunriseEpoch { get; set; }
        public string Sunset { get; set; }
        public int SunsetEpoch { get; set; }
        public float Moonphase { get; set; }
    }

    public class Day
    {
        public string Datetime { get; set; }
        public int DatetimeEpoch { get; set; }
        public float Tempmax { get; set; }
        public float Tempmin { get; set; }
        public float Temp { get; set; }
        public float Feelslikemax { get; set; }
        public float Feelslikemin { get; set; }
        public float Feelslike { get; set; }
        public float Dew { get; set; }
        public float Humidity { get; set; }
        public double Precip { get; set; }
        public double Precipprob { get; set; }
        public double Precipcover { get; set; }
        public object Preciptype { get; set; }
        public double Snow { get; set; }
        public double Snowdepth { get; set; }
        public double Windgust { get; set; }
        public float Windspeed { get; set; }
        public double Winddir { get; set; }
        public float Pressure { get; set; }
        public float Cloudcover { get; set; }
        public float Visibility { get; set; }
        public float Solarradiation { get; set; }
        public float Solarenergy { get; set; }
        public double Uvindex { get; set; }
        public double Severerisk { get; set; }
        public string Sunrise { get; set; }
        public int SunriseEpoch { get; set; }
        public string Sunset { get; set; }
        public int SunsetEpoch { get; set; }
        public float Moonphase { get; set; }
        public string Conditions { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        //public string[] stations { get; set; }
        //public string source { get; set; }
        public Hour[] hours { get; set; }
    }

    public class Hour
    {
        public string Datetime { get; set; }
        public int DatetimeEpoch { get; set; }
        public float Temp { get; set; }
        public float Feelslike { get; set; }
        public float Humidity { get; set; }
        public float Dew { get; set; }
        public double Precip { get; set; }
        public double Precipprob { get; set; }
        public double Snow { get; set; }
        public double Snowdepth { get; set; }
        public object Preciptype { get; set; }
        public float Windgust { get; set; }
        public float Windspeed { get; set; }
        public float Winddir { get; set; }
        public double Pressure { get; set; }
        public float Visibility { get; set; }
        public float Cloudcover { get; set; }
        public double Solarradiation { get; set; }
        public float Solarenergy { get; set; }
        public double Uvindex { get; set; }
        public double Severerisk { get; set; }
        public string Conditions { get; set; }
        public string Icon { get; set; }
        //public string[] stations { get; set; }
        //public string source { get; set; }
    }
}
