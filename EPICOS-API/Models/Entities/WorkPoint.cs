using System;
using System.Collections.Generic;

namespace EPICOS_API.Models.Entities
{
    public class Workpoint : Device
    {
        public double CoordinateX { get; set; }
        public double CoordinateY { get; set; }
        public double CoordinateZ { get; set; }
        public int OfficeID { get; set; }
        public int FloorID { get; set; }
        public int HubID { get; set; }
        public int Rotation { get; set; }
        public int LocationType { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Hub Hub { get; set; }
        public Floor Floor { get; set; }

        public enum Types
        {
            WorkStation = 1,
            Rooms = 2,
        }
    }
}