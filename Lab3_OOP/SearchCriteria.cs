﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3_OOP
{
    internal class SearchCriteria
    {
        public SearchCriteria()
        {
            AuthorName = "";
            Faculty = "";
            Department = "";
        }

        public string AuthorName { get; set; }
        public string Faculty { get; set; }
        public string Department { get; set; }
    }
}
