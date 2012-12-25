using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace CinguettioDataService.Models
{
    [DataContract]
    public class PostModel
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Content { get; set; }

        [DataMember]
        public DateTime DateCreated { get; set; }

        [DataMember]
        public UserModel User { get; set; }
    }
}