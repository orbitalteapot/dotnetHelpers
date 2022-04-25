using System;
using System.Collections.Generic;
// ReSharper disable All
#pragma warning disable IDE1006

namespace OrbitalTeapot.RestApi.Models.Met
{

    public class Geometry
    {
        public string? type { get; set; }
        public List<double>? coordinates { get; set; }
    }

    public class Units
    {
        public string? air_pressure_at_sea_level { get; set; }
        public string? air_temperature { get; set; }
        public string? cloud_area_fraction { get; set; }
        public string? precipitation_amount { get; set; }
        public string? relative_humidity { get; set; }
        public string? wind_from_direction { get; set; }
        public string? wind_speed { get; set; }
    }

    public class Meta
    {
        public DateTime updated_at { get; set; }
        public Units? units { get; set; }
    }

    public class Details
    {
        public double? air_pressure_at_sea_level { get; set; }
        public double? air_temperature { get; set; }
        public double? cloud_area_fraction { get; set; }
        public double? relative_humidity { get; set; }
        public double? wind_from_direction { get; set; }
        public double? wind_speed { get; set; }
        public double? precipitation_amount { get; set; }
    }

    public class Instant
    {
        public Details? details { get; set; }
    }

    public class Summary
    {
        public string? symbol_code { get; set; }
    }

    public class Next12Hours
    {
        public Summary? summary { get; set; }
    }

    public class Next1Hours
    {
        public Summary? summary { get; set; }
        public Details? details { get; set; }
    }

    public class Next6Hours
    {
        public Summary? summary { get; set; }
        public Details? details { get; set; }
    }

    public class Data
    {
        public Instant? instant { get; set; }
        public Next12Hours? next_12_hours { get; set; }
        public Next1Hours? next_1_hours { get; set; }
        public Next6Hours? next_6_hours { get; set; }
    }

    public class Timesery
    {
        public DateTime time { get; set; }
        public Data? data { get; set; }
    }

    public class Properties
    {
        public Meta? meta { get; set; }
        public List<Timesery>? timeseries { get; set; }
    }

    public class Yr
    {
        public string? type { get; set; }
        public Geometry? geometry { get; set; }
        public Properties? properties { get; set; }
    }
}
