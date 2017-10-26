using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Karenkof
{
    public class datacurrency
    {
        private string _id;
        [JsonProperty(PropertyName = "id")]
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _currencyfrom;
        [JsonProperty(PropertyName = "currencyfrom")]
        public string Currencyfrom
        {
            get { return _currencyfrom; }
            set { _currencyfrom = value; }
        }

        private string _currencyto;
        [JsonProperty(PropertyName = "currencyto")]
        public string Currencyto
        {
            get { return _currencyto; }
            set { _currencyto = value; }
        }

        private string _nominalfrom;
        [JsonProperty(PropertyName = "nominalfrom")]
        public string Nominalfrom
        {
            get { return _nominalfrom; }
            set { _nominalfrom = value; }
        }

        private string _nominalto;
        [JsonProperty(PropertyName = "nominalto")]
        public string Nominalto
        {
            get { return _nominalto; }
            set { _nominalto = value; }
        }
    }
}
