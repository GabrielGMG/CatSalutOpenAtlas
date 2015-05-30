using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CatSalutOpenAtlas.Models
{
    public class Feature
    {
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        private string desc;

        public string Desc
        {
            get { return desc; }
            set { desc = value; }
        }
        private string color;

        public string Color
        {
            get { return color; }
            set { color = value; }
        }

        public Feature(int pId, string pDesc, string pColor)
        {
            this.Id = pId;
            this.Desc = pDesc;
            this.Color = pColor;
        }
    }
}