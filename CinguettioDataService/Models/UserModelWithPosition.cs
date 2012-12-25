using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace CinguettioDataService.Models
{
    [DataContract]
    public class UserModelWithPosition : UserModel
    {
        [DataMember]
        public decimal Latitude { get; set; }

        [DataMember]
        public decimal Longitude { get; set; }
    }
}