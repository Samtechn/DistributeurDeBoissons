using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistributeurBoisson.Models
{
    public class Recipe
    {
        public string Name { get; set; }
        public Dictionary<string,int> DicComposition { get; set; }
    }
}
