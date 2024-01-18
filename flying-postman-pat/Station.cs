using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flying_postman_pat
{
    class Station
    {
        //Property
        public List<string> name { get; set; }
        public List<int> xcoord { get; set; }
        public List<int> ycoord { get; set; }

        //Instance Constructor
        public Station()
        {
            name = new List<string>();
            xcoord = new List<int>();
            ycoord = new List<int>();
        }

    }
}
