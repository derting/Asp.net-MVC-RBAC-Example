﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RBACExample.Models
{
    public class ControllerAction
    {
        public string Controller { get; set; }
        public string ControllerLabel { get; set; }
        public string Action { get; set; }
        public string ActionLabel { get; set; }
    }
}