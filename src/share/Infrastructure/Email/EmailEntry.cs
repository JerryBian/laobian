﻿using System;
using System.Collections.Generic;
using System.Text;
using Laobian.Share.Config;

namespace Laobian.Share.Infrastructure.Email
{
    public class EmailEntry
    {
        public EmailEntry(string toName, string toAddress)
        {
            ToName = toName;
            ToAddress = toAddress;
        }

        public string FromName { get; set; }

        public string FromAddress { get; set; }

        public string ToName { get; set; }

        public string ToAddress { get; set; }

        public string Subject { get; set; }

        public string PlainContent { get; set; }

        public string HtmlContent { get; set; }
    }
}