using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NadekoBot.Classes.CardsAgainstHumanity
{
    public class CAHBlackCard
    {
        public string Category;
        public string CardContents;
        public int WhiteCards;

        public CAHBlackCard(string contents, string category = "", int wc = 1)
        {
            CardContents = contents;
            Category = category;
            WhiteCards = wc;
        }

        public override string ToString()
        {
            string str;
            if (WhiteCards >1)
            {
                return $"*{CardContents}*\n **CHOOSE {WhiteCards} CARDS**";
            }

            if (Category != String.Empty)
            {
                str = $"**{Category}**:\n*{CardContents}*";
            } else
            {
                str = $"*{CardContents}*";
            }
            

            return str;
        }
    }
}
