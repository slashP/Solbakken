﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Security;

namespace CodeFirstMembershipSharp
{ 
    public class DataContextInitializer:DropCreateDatabaseAlways<DataContext>
    {
        protected override void Seed(DataContext context)
        {
        }
    }
}