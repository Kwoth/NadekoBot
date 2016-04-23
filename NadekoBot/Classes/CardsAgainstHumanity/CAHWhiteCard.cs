using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NadekoBot.Classes.CardsAgainstHumanity
{
    public class CAHWhiteCard
    {
        public string CardContents;
        public string Category;
        public CAHWhiteCard(string contents, string category = "")
        {
            Category = category;
            CardContents = contents;
        }

        public override string ToString()
        {
            return $"**{CardContents}**";
        }
    }
}
