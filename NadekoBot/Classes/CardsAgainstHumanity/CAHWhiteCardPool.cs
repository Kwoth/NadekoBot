using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NadekoBot.Classes.CardsAgainstHumanity
{
    class CAHWhiteCardPool
    {
        public static CAHWhiteCardPool Instance { get; } = new CAHWhiteCardPool();

        public HashSet<CAHWhiteCard> pool = new HashSet<CAHWhiteCard>();

        private Random rng { get; } = new Random();

        static CAHWhiteCardPool() { }

        private CAHWhiteCardPool()
        {
            Reload();
        }

        public CAHWhiteCard GetRandomWhiteCard(IEnumerable<CAHWhiteCard> exclude)
        {
            var list = pool.Except(exclude).ToList();
            var rand = rng.Next(0, list.Count);
            return list[rand];
        }

        internal void Reload()
        {
            var arr = JArray.Parse(File.ReadAllText("data/whitecards.json"));

            foreach (var item in arr)
            {
                var wc = new CAHWhiteCard(item["Contents"].ToString());
                pool.Add(wc);
            }
            var r = new Random();
            pool = new HashSet<CAHWhiteCard>(pool.OrderBy(x => r.Next()));
        }
    }
}
