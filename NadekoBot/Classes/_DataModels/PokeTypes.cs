﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NadekoBot.Classes._DataModels
{
    class userPokeTypes : IDataModel
    {
        public long UserId { get; set; }
        public int type { get; set; }
    }
}
