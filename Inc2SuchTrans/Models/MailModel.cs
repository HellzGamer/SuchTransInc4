using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Inc2SuchTrans.Models
{
    public class MailModel
    {
        public string From { get; set; }

        [Required(ErrorMessage = "Please enter a valid email address")]
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }



    public class modelsWrapper
    {
        public MailModel mailModel { get; set; }

        public Quote quote { get; set; }
    }
}