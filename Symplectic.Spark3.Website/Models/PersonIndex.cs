using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Symplectic.Spark3.Website.Models
{
    public class PersonIndex :AbstractModel
    {
        public PersonIndex()
        {
            this.Id = "index";
        }

        public override string ModelName
        {
            get { return "person"; }
        }

        public override string ViewName
        {
            get { return "person-index"; }
        }
    }
}