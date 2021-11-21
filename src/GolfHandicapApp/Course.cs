using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Course
{
    public class Course
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public List<Tee> Tees { get; set; }
    }

    public class Tee
    {
        public string Name { get; set; }
        public double Slope { get; set; }
        public double Rating { get; set; }
    }
}