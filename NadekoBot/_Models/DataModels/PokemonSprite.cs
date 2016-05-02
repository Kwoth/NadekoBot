namespace NadekoBot.DataModels
{
    class PokemonSprite : IDataModel
    {
        public long OwnerId { get; set; }
        public string NickName { get; set; } 
        public bool IsShiny { get; set; } = false;
        public int HP { get; set; }
        public long XP { get; set; }
        public int Level { get; set; }
        public int SpeciesId { get; set; }
        public bool IsActive { get; set; }
            
        //stats
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int MaxHP { get; set; }
        public int Speed { get; set; }
        public int SpecialAttack { get; set; }
        public int SpecialDefense { get; set; }


    }
}
