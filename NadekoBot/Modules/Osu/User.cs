namespace NadekoBot.Modules.Osu
{
    public class User
    {
        public int user_id { get; set; }
        public string username { get; set; }
        public int count300 { get; set; }
        public int count100 { get; set; }
        public int count50 { get; set; }
        public int playcount { get; set; }
        public long ranked_score { get; set; }
        public long total_score { get; set; }
        public int pp_rank { get; set; }
        public double level { get; set; }
        public double pp_raw { get; set; }
        public float accuracy { get; set; }
        public int count_rank_ss { get; set; }
        public int count_rank_s { get; set; }
        public int count_rank_a { get; set; }
        public string country { get; set; }
    }
}
