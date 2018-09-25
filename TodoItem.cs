using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2
{
    public class TodoItem
    {
        public Nullable<int> ID { get; set; }
        public string UserEmail { get; set; }
        public string Item { get; set; }
        public Nullable<int> Percentage { get; set; }
    }
}