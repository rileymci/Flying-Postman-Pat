using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flying_postman_pat
{
    class Plane
    {

        public double range { get; set; }
        public double speed { get; set; }
        public int takeOffTime { get; set; }
        public int landingTime { get; set; }
        public int refuelTime { get; set; }

        private Plane()
        {// private so that no other instances of it
        }
        public static Plane plane = new Plane();

    }
}
