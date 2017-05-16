using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal.Data
{
    public class Article
    {
        public ObjectId _id { get; set; }
        public string naziv { get; set; }
        public string sadrzaj { get; set; }
        public string autor { get; set; }

        public IList<string> komentari = new List<string>();
        public byte[] link { get; set; }

    }
}