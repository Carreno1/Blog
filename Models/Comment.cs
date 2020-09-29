using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Portfolio_Blog.Models
{
    public class Comment
    {
        //The PK of the comment
        public int Id { get; set; }

        //This is the PK of the BLogPost entry that this comment belongs to
        public int BlogPostId { get; set; }


        //What is the PK of the User that wrote this Comment
        public string AuthorId { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        public string UpdatedReason { get; set; }

        public string Body { get; set; }


        //Navigational properties
        public virtual BlogPost Blog { get; set; }
        public virtual ApplicationUser Author { get; set; }








    }
}