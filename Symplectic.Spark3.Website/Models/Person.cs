using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;



namespace Symplectic.Spark3.Website.Models
{
    public class Person : AbstractModel
    {
       
        public override string ModelName
        {
            get { return "person"; }
        }

        public override string ViewName
        {
            get { return "person"; }
        }
       
    }
}