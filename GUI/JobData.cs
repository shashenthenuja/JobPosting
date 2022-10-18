﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI
{
    public class JobData
    {
        public int JobId { get; set; }
        public string code { get; set; }
        public string status { get; set; }
        public string result { get; set; }
        public JobData()
        {
            JobId = 0;
            code = "";
            status = "";
            result = "";
        }
    }
}