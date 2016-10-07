using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NadekoBot.Services.Database.Models
{
    public class CustomReaction : DbEntity
    {
        public long? ServerId { get; set; }
        [NotMapped]
        public Regex Regex { get; set; }
        public List<Response> Responses { get; set; }
        public string Trigger { get; set; }
        public bool IsRegex { get; set; }
        public override string ToString() => $"Id: {Id}\nTrigger: {Trigger}\n Regex: {IsRegex}\n Global: {ServerId == null}\n Responses: {string.Join("\n-", Responses.Select(r => r.Text))}";
        public string ToStringWithoutResponses() => $"Id: {Id}\nTrigger: {Trigger}\nRegex: {IsRegex}\nGlobal: {ServerId == null}";

    }


    public class Response : DbEntity
    {
        public string Text { get; set; }
    }

   
}
