using diplom.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace diplom.Models
{
    public class CandlesByDay
    {
        public double? Open { get; set; }
        public double? Close { get; set; }
        public double? High { get; set; }
        public double? Low { get; set; }
        public string? Day { get; set; }
        //public DateTime Time
        //{
        //    get { return Time; }
        //    set { Time = DateTime.Parse(value.ToString()); }
        //}
        public long? Volume { get; set; }
        public long? ShareId { get; set; }
    }
}
