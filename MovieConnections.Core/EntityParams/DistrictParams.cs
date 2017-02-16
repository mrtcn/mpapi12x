﻿using MovieConnections.Core.Model;
using MovieConnections.Data.Entities;
using MovieConnections.Data.Models;

namespace MovieConnections.Core.EntityParams {
    public class DistrictParams : IDistrict, IEntityParams {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsOverpopulated { get; set; }
        public int CityId { get; set; }
        public Status Status { get; set; }
    }
}