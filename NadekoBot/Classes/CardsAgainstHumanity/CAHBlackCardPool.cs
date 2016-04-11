using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NadekoBot.Classes.CardsAgainstHumanity
{
    class CAHBlackCardPool
    {
        public static CAHBlackCardPool Instance { get; } = new CAHBlackCardPool();

        public HashSet<CAHBlackCard> pool = new HashSet<CAHBlackCard>();

        private Random rng { get; } = new Random();

        static CAHBlackCardPool() { }

        private CAHBlackCardPool()
        {
            Reload();
        }

        public CAHBlackCard GetRandomBlackCard(IEnumerable<CAHBlackCard> exclude)
        {
            //var list=  pool.Where(x => x.WhiteCards > 1).ToList();
            //return t[1];

            var list = pool.Except(exclude).ToList();
            var rand = rng.Next(0, list.Count);
            return list[rand];
        }

        internal void Reload()
        {
            var arr = JArray.Parse(File.ReadAllText("data/blackcards.json"));

            foreach (var item in arr)
            {
                var bc = new CAHBlackCard(item["Contents"].ToString(), item["Category"].ToString(), int.Parse(item["PickAmount"].ToString()));
                pool.Add(bc);
            }
            var r = new Random();
            pool = new HashSet<CAHBlackCard>(pool.OrderBy(x => r.Next()));
        }
    }

}
