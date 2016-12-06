using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace NadekoBot.Modules.Searches.Models
{
    public class OverwatchApiModel
    {

        //patch notes
        public OverwatchPatchNotes[] PatchNotes { get; set; }
        public OverwatchPagination[] Pagination { get; set; }

        //Player All Heroes Stats
        public OverwatchAllHeroes[] AllHeroes { get; set; }

        //Player Hero Stats
        public OverwatchHero Hero { get; set; }

        //achievements
        public OverwatchAchievements[] Achievements { get; set; }
        public float totalNumberOfAchievements { get; set; }
        public float numberOfAchievementsCompleted { get; set; }
        public string finishedAchievements { get; set; }

        public OverwatchPlayer Data { get; set; }

        public class OverwatchHero
        {
            public Hero_Genji Genji { get; set; }
            public Hero_Mccree Mccree { get; set; }
            public Hero_Pharah Pharah { get; set; }
            public Hero_Reaper Reaper { get; set; }
            public Hero_Soldier76 Soldier76 { get; set; }
            public Hero_Tracer Tracer { get; set; }
            public Hero_Bastion Bastion { get; set; }
            public Hero_Hanzo Hanzo { get; set; }
            public Hero_Junkrat Junkrat { get; set; }
            public Hero_Mei Mei { get; set; }
            public Hero_Tobjoern Tobjoern { get; set; }
            public Hero_Widowmaker Widowmaker { get; set; }
            public Hero_DVa DVa { get; set; }
            public Hero_Reinhardt Reinhardt { get; set; }
            public Hero_Roadhog Roadhog { get; set; }
            public Hero_Winston Winston { get; set; }
            public Hero_Zarya Zarya { get; set; }
            public Hero_Ana Ana { get; set; }
            public class Hero_Ana
            {

            }
            public class Hero_Zarya
            {
                public string DamageBlocked { get; set; }
                public string DamageBlocked_MostinGame { get; set; }
                public string LifetimeGravitonSurgeKills { get; set; }
                public string GravitonSurgeKills_MostinGame { get; set; }
                public string HighEnergyKills_MostinGame { get; set; }
                public string HighEnergyKills { get; set; }
                public string LifetimeEnergyAccumulation { get; set; }
                public string EnergyMaximum { get; set; }
                public string ProjectedBarriersApplied { get; set; }
                public string AverageEnergy_BestinGame { get; set; }
                public string MeleeFinalBlows_MostinGame { get; set; }
                public string ProjectedBarriersApplied_Average { get; set; }
                public string HighEnergyKills_Average { get; set; }
                public string GravitonSurgeKills_Average { get; set; }
                public string DamageBlocked_Average { get; set; }
                public string LifetimeAverageEnergy { get; set; }
                public string Eliminations { get; set; }
                public string FinalBlows { get; set; }
                public string SoloKills { get; set; }
                public string ShotsFired { get; set; }
                public string ShotsHit { get; set; }
                public string DamageDone { get; set; }
                public string ObjectiveKills { get; set; }
                public string Multikills { get; set; }
                public string MeleeFinalBlows { get; set; }
                public string EliminationsperLife { get; set; }
                public string WeaponAccuracy { get; set; }
                public string TurretsDestroyed { get; set; }
                public string Eliminations_MostinLife { get; set; }
                public string DamageDone_MostinLife { get; set; }
                public string WeaponAccuracy_BestinGame { get; set; }
                public string KillStreak_Best { get; set; }
                public string DamageDone_MostinGame { get; set; }
                public string Eliminations_MostinGame { get; set; }
                public string FinalBlows_MostinGame { get; set; }
                public string ObjectiveKills_MostinGame { get; set; }
                public string ObjectiveTime_MostinGame { get; set; }
                public string SoloKills_MostinGame { get; set; }
                public string MeleeFinalBlows_Average { get; set; }
                public string Deaths_Average { get; set; }
                public string SoloKills_Average { get; set; }
                public string ObjectiveTime_Average { get; set; }
                public string ObjectiveKills_Average { get; set; }
                public string FinalBlows_Average { get; set; }
                public string Eliminations_Average { get; set; }
                public string DamageDone_Average { get; set; }
                public string Deaths { get; set; }
                public string EnvironmentalDeaths { get; set; }
                public string Medals_Bronze { get; set; }
                public string Medals_Silver { get; set; }
                public string Medals_Gold { get; set; }
                public string Medals { get; set; }
                public string Cards { get; set; }
                public string TimePlayed { get; set; }
                public string GamesWon { get; set; }
                public string ObjectiveTime { get; set; }
                public string TimeSpentonFire { get; set; }
                public string Multikill_Best { get; set; }
                public string ProjectedBarriersApplied_MostinGame { get; set; }
            }
            public class Hero_Winston
            {
                public string PlayersKnockedBack { get; set; }
                public string DamageBlocked { get; set; }
                public string DamageBlocked_MostinGame { get; set; }
                public string PlayersKnockedBack_MostinGame { get; set; }
                public string MeleeKills { get; set; }
                public string MeleeKills_MostinGame { get; set; }
                public string JumpPackKills { get; set; }
                public string JumpPackKills_MostinGame { get; set; }
                public string MeleeFinalBlows_MostinGame { get; set; }
                public string PlayersKnockedBack_Average { get; set; }
                public string MeleeKills_Average { get; set; }
                public string JumpPackKills_Average { get; set; }
                public string DamageBlocked_Average { get; set; }
                public string Eliminations { get; set; }
                public string FinalBlows { get; set; }
                public string SoloKills { get; set; }
                public string DamageDone { get; set; }
                public string ObjectiveKills { get; set; }
                public string Multikills { get; set; }
                public string EnvironmentalKills { get; set; }
                public string MeleeFinalBlows { get; set; }
                public string EliminationsperLife { get; set; }
                public string TeleporterPadsDestroyed { get; set; }
                public string TurretsDestroyed { get; set; }
                public string Eliminations_MostinLife { get; set; }
                public string DamageDone_MostinLife { get; set; }
                public string KillStreak_Best { get; set; }
                public string DamageDone_MostinGame { get; set; }
                public string Eliminations_MostinGame { get; set; }
                public string FinalBlows_MostinGame { get; set; }
                public string ObjectiveKills_MostinGame { get; set; }
                public string ObjectiveTime_MostinGame { get; set; }
                public string SoloKills_MostinGame { get; set; }
                public string MeleeFinalBlows_Average { get; set; }
                public string Deaths_Average { get; set; }
                public string SoloKills_Average { get; set; }
                public string ObjectiveTime_Average { get; set; }
                public string ObjectiveKills_Average { get; set; }
                public string FinalBlows_Average { get; set; }
                public string Eliminations_Average { get; set; }
                public string DamageDone_Average { get; set; }
                public string Deaths { get; set; }
                public string EnvironmentalDeaths { get; set; }
                public string Medals_Bronze { get; set; }
                public string Medals_Silver { get; set; }
                public string Medals_Gold { get; set; }
                public string Medals { get; set; }
                public string Cards { get; set; }
                public string TimePlayed { get; set; }
                public string GamesWon { get; set; }
                public string ObjectiveTime { get; set; }
                public string TimeSpentonFire { get; set; }
                public string Multikill_Best { get; set; }
                public string PrimalRageKills { get; set; }
                public string PrimalRageKills_MostinGame { get; set; }
                public string PrimalRageKills_Average { get; set; }
            }
            public class Hero_Roadhog
            {
                [JsonProperty("EnemiesHooked_MostinGame")]
                public string EnemiesHooked_MostinGame { get; set; }
                public string EnemiesHooked { get; set; }
                public string HooksAttempted { get; set; }
                [JsonProperty("WholeHogKills_MostinGame")]
                public string WholeHogKills_MostinGame { get; set; }
                public string WholeHogKills { get; set; }
                [JsonProperty("HookAccuracy_BestinGame")]
                public string HookAccuracy_BestinGame { get; set; }
                [JsonProperty("MeleeFinalBlows_MostinGame")]
                public string MeleeFinalBlows_MostinGame { get; set; }
                [JsonProperty("WholeHogKills_Average")]
                public string WholeHogKills_Average { get; set; }
                [JsonProperty("EnemiesHooked_Average")]
                public string EnemiesHooked_Average { get; set; }
                public string HookAccuracy { get; set; }
                public string Eliminations { get; set; }
                public string FinalBlows { get; set; }
                public string SoloKills { get; set; }
                public string ShotsFired { get; set; }
                public string ShotsHit { get; set; }
                public string CriticalHits { get; set; }
                public string DamageDone { get; set; }
                public string ObjectiveKills { get; set; }
                public string Multikills { get; set; }
                public string EnvironmentalKills { get; set; }
                public string MeleeFinalBlows { get; set; }
                public string CriticalHitsperMinute { get; set; }
                public string CriticalHitAccuracy { get; set; }
                public string EliminationsperLife { get; set; }
                public string WeaponAccuracy { get; set; }
                public string HealingDone { get; set; }
                public string TeleporterPadsDestroyed { get; set; }
                public string TurretsDestroyed { get; set; }
                public string SelfHealing { get; set; }
                [JsonProperty("Eliminations_MostinLife")]
                public string Eliminations_MostinLife { get; set; }
                [JsonProperty("DamageDone_MostinLife")]
                public string DamageDone_MostinLife { get; set; }
                [JsonProperty("HealingDone_MostinLife")]
                public string HealingDone_MostinLife { get; set; }
                [JsonProperty("WeaponAccuracy_BestinGame")]
                public string WeaponAccuracy_BestinGame { get; set; }
                [JsonProperty("KillStreak_Best")]
                public string KillStreak_Best { get; set; }
                [JsonProperty("DamageDone_MostinGame")]
                public string DamageDone_MostinGame { get; set; }
                [JsonProperty("HealingDone_MostinGame")]
                public string HealingDone_MostinGame { get; set; }
                [JsonProperty("Eliminations_MostinGame")]
                public string Eliminations_MostinGame { get; set; }
                [JsonProperty("FinalBlows_MostinGame")]
                public string FinalBlows_MostinGame { get; set; }
                [JsonProperty("ObjectiveKills_MostinGame")]
                public string ObjectiveKills_MostinGame { get; set; }
                [JsonProperty("ObjectiveTime_MostinGame")]
                public string ObjectiveTime_MostinGame { get; set; }
                [JsonProperty("SoloKills_MostinGame")]
                public string SoloKills_MostinGame { get; set; }
                [JsonProperty("CriticalHits_MostinGame")]
                public string CriticalHits_MostinGame { get; set; }
                [JsonProperty("CriticalHits_MostinLife")]
                public string CriticalHits_MostinLife { get; set; }
                [JsonProperty("SelfHealing_MostinGame")]
                public string SelfHealing_MostinGame { get; set; }
                [JsonProperty("MeleeFinalBlows_Average")]
                public string MeleeFinalBlows_Average { get; set; }
                [JsonProperty("SelfHealing_Average")]
                public string SelfHealing_Average { get; set; }
                [JsonProperty("Deaths_Average")]
                public string Deaths_Average { get; set; }
                [JsonProperty("SoloKills_Average")]
                public string SoloKills_Average { get; set; }
                [JsonProperty("ObjectiveTime_Average")]
                public string ObjectiveTime_Average { get; set; }
                [JsonProperty("ObjectiveKills_Average")]
                public string ObjectiveKills_Average { get; set; }
                [JsonProperty("HealingDone_Average")]
                public string HealingDone_Average { get; set; }
                [JsonProperty("FinalBlows_Average")]
                public string FinalBlows_Average { get; set; }
                [JsonProperty("Eliminations_Average")]
                public string Eliminations_Average { get; set; }
                [JsonProperty("DamageDone_Average")]
                public string DamageDone_Average { get; set; }
                public string Deaths { get; set; }
                public string EnvironmentalDeaths { get; set; }
                [JsonProperty("Medals-Bronze")]
                public string Medals_Bronze { get; set; }
                [JsonProperty("Medals-Silver")]
                public string Medals_Silver { get; set; }
                [JsonProperty("Medals-Gold")]
                public string Medals_Gold { get; set; }
                public string Medals { get; set; }
                public string Cards { get; set; }
                public string TimePlayed { get; set; }
                public string GamesWon { get; set; }
                public string ObjectiveTime { get; set; }
                public string TimeSpentonFire { get; set; }
                [JsonProperty("Multikill-Best")]
                public string Multikill_Best { get; set; }
            }
            public class Hero_Reinhardt
            {
                public string DamageBlocked { get; set; }
                [JsonProperty("DamageBlocked_MostinGame")]
                public string DamageBlocked_MostinGame { get; set; }
                public string ChargeKills { get; set; }
                [JsonProperty("ChargeKills_MostinGame")]
                public string ChargeKills_MostinGame { get; set; }
                public string FireStrikeKills { get; set; }
                [JsonProperty("FireStrikeKills_MostinGame")]
                public string FireStrikeKills_MostinGame { get; set; }
                public string EarthshatterKills { get; set; }
                [JsonProperty("EarthshatterKills_MostinGame")]
                public string EarthshatterKills_MostinGame { get; set; }
                [JsonProperty("FireStrikeKills_Average")]
                public string FireStrikeKills_Average { get; set; }
                [JsonProperty("EarthshatterKills_Average")]
                public string EarthshatterKills_Average { get; set; }
                [JsonProperty("DamageBlocked_Average")]
                public string DamageBlocked_Average { get; set; }
                [JsonProperty("ChargeKills_Average")]
                public string ChargeKills_Average { get; set; }
                public string Eliminations { get; set; }
                public string FinalBlows { get; set; }
                public string SoloKills { get; set; }
                public string DamageDone { get; set; }
                public string ObjectiveKills { get; set; }
                public string Multikills { get; set; }
                public string EnvironmentalKill { get; set; }
                public string EliminationsperLife { get; set; }
                public string TurretsDestroyed { get; set; }
                [JsonProperty("Eliminations_MostinLife")]
                public string Eliminations_MostinLife { get; set; }
                [JsonProperty("DamageDone_MostinLife")]
                public string DamageDone_MostinLife { get; set; }
                [JsonProperty("KillStreak_Best")]
                public string KillStreak_Best { get; set; }
                [JsonProperty("DamageDone_MostinGame")]
                public string DamageDone_MostinGame { get; set; }
                [JsonProperty("Eliminations_MostinGame")]
                public string Eliminations_MostinGame { get; set; }
                [JsonProperty("FinalBlows_MostinGame")]
                public string FinalBlows_MostinGame { get; set; }
                [JsonProperty("ObjectiveKills_MostinGame")]
                public string ObjectiveKills_MostinGame { get; set; }
                [JsonProperty("ObjectiveTime_MostinGame")]
                public string ObjectiveTime_MostinGame { get; set; }
                [JsonProperty("SoloKills_MostinGame")]
                public string SoloKills_MostinGame { get; set; }
                [JsonProperty("Deaths_Average")]
                public string Deaths_Average { get; set; }
                [JsonProperty("SoloKills_Average")]
                public string SoloKills_Average { get; set; }
                [JsonProperty("ObjectiveTime_Average")]
                public string ObjectiveTime_Average { get; set; }
                [JsonProperty("ObjectiveKills_Average")]
                public string ObjectiveKills_Average { get; set; }
                [JsonProperty("FinalBlows_Average")]
                public string FinalBlows_Average { get; set; }
                [JsonProperty("Eliminations_Average")]
                public string Eliminations_Average { get; set; }
                [JsonProperty("DamageDone_Average")]
                public string DamageDone_Average { get; set; }
                public string Deaths { get; set; }
                public string EnvironmentalDeaths { get; set; }
                [JsonProperty("Medals-Bronze")]
                public string Medals_Bronze { get; set; }
                [JsonProperty("Medals-Silver")]
                public string Medals_Silver { get; set; }
                [JsonProperty("Medals-Gold")]
                public string Medals_Gold { get; set; }
                public string Medals { get; set; }
                public string Cards { get; set; }
                public string TimePlayed { get; set; }
                public string GamesWon { get; set; }
                public string ObjectiveTime { get; set; }
                public string TimeSpentonFire { get; set; }
                [JsonProperty("Multikill_Best")]
                public string Multikill_Best { get; set; }
            }
            public class Hero_DVa
            {
                [JsonProperty("MechsCalled")]
                public string MechsCalled { get; set; }
                [JsonProperty("MechsCalled-MostinGame")]
                public string MechsCalled_MostinGame { get; set; }
                [JsonProperty("DamageBlocked-MostinGame")]
                public string DamageBlocked_MostinGame { get; set; }
                [JsonProperty("DamageBlocked")]
                public string DamageBlocked { get; set; }
                [JsonProperty("MechDeaths")]
                public string MechDeaths { get; set; }
                [JsonProperty("MeleeFinalBlows-MostinGame")]
                public string MeleeFinalBlows_MostinGame { get; set; }
                [JsonProperty("MechsCalled-Average")]
                public string MechsCalled_Average { get; set; }
                [JsonProperty("DamageBlocked-Average")]
                public string DamageBlocked_Average { get; set; }
                [JsonProperty("Eliminations")]
                public string Eliminations { get; set; }
                [JsonProperty("FinalBlows")]
                public string FinalBlows { get; set; }
                [JsonProperty("SoloKills")]
                public string SoloKills { get; set; }
                [JsonProperty("ShotsFired")]
                public string ShotsFired { get; set; }
                [JsonProperty("ShotsHit")]
                public string ShotsHit { get; set; }
                [JsonProperty("CriticalHits")]
                public string CriticalHits { get; set; }
                [JsonProperty("DamageDone")]
                public string DamageDone { get; set; }
                [JsonProperty("ObjectiveKills")]
                public string ObjectiveKills { get; set; }
                [JsonProperty("Multikills")]
                public string Multikills { get; set; }
                [JsonProperty("EnvironmentalKills")]
                public string EnvironmentalKills { get; set; }
                [JsonProperty("MeleeFinalBlows")]
                public string MeleeFinalBlows { get; set; }
                [JsonProperty("CriticalHitsperMinute")]
                public string CriticalHitsperMinute { get; set; }
                [JsonProperty("CriticalHitAccuracy")]
                public string CriticalHitAccuracy { get; set; }
                [JsonProperty("EliminationsperLife")]
                public string EliminationsperLife { get; set; }
                [JsonProperty("WeaponAccuracy")]
                public string WeaponAccuracy { get; set; }
                [JsonProperty("TurretsDestroyed")]
                public string TurretsDestroyed { get; set; }
                [JsonProperty("Eliminations-MostinLife")]
                public string Eliminations_MostinLife { get; set; }
                [JsonProperty("DamageDone-MostinLife")]
                public string DamageDone_MostinLife { get; set; }
                [JsonProperty("WeaponAccuracy-BestinGame")]
                public string WeaponAccuracy_BestinGame { get; set; }
                [JsonProperty("KillStreak-Best")]
                public string KillStreak_Best { get; set; }
                [JsonProperty("DamageDone-MostinGame")]
                public string DamageDone_MostinGame { get; set; }
                [JsonProperty("Eliminations-MostinGame")]
                public string Eliminations_MostinGame { get; set; }
                [JsonProperty("FinalBlows-MostinGame")]
                public string FinalBlows_MostinGame { get; set; }
                [JsonProperty("ObjectiveKills-MostinGame")]
                public string ObjectiveKills_MostinGame { get; set; }
                [JsonProperty("ObjectiveTime-MostinGame")]
                public string ObjectiveTime_MostinGame { get; set; }
                [JsonProperty("SoloKills-MostinGame")]
                public string SoloKills_MostinGame { get; set; }
                [JsonProperty("CriticalHits-MostinGame")]
                public string CriticalHits_MostinGame { get; set; }
                [JsonProperty("CriticalHits-MostinLife")]
                public string CriticalHits_MostinLife { get; set; }
                [JsonProperty("MeleeFinalBlows-Average")]
                public string MeleeFinalBlows_Average { get; set; }
                [JsonProperty("Deaths-Average")]
                public string Deaths_Average { get; set; }
                [JsonProperty("SoloKills-Average")]
                public string SoloKills_Average { get; set; }
                [JsonProperty("ObjectiveTime-Average")]
                public string ObjectiveTime_Average { get; set; }
                [JsonProperty("ObjectiveKills-Average")]
                public string ObjectiveKills_Average { get; set; }
                [JsonProperty("FinalBlows-Average")]
                public string FinalBlows_Average { get; set; }
                [JsonProperty("Eliminations-Average")]
                public string Eliminations_Average { get; set; }
                [JsonProperty("DamageDone-Average")]
                public string DamageDone_Average { get; set; }
                [JsonProperty("Deaths")]
                public string Deaths { get; set; }
                [JsonProperty("EnvironmentalDeaths")]
                public string EnvironmentalDeaths { get; set; }
                [JsonProperty("Medals-Bronze")]
                public string Medals_Bronze { get; set; }
                [JsonProperty("Medals-Silver")]
                public string Medals_Silver { get; set; }
                [JsonProperty("Medals-Gold")]
                public string Medals_Gold { get; set; }
                [JsonProperty("Medals")]
                public string Medals { get; set; }
                [JsonProperty("Cards")]
                public string Cards { get; set; }
                [JsonProperty("TimePlayed")]
                public string TimePlayed { get; set; }
                [JsonProperty("GamesWon")]
                public string GamesWon { get; set; }
                [JsonProperty("ObjectiveTime")]
                public string ObjectiveTime { get; set; }
                [JsonProperty("TimeSpentonFire")]
                public string TimeSpentonFire { get; set; }
                [JsonProperty("Self-DestructKills")]
                public string Self_DestructKills { get; set; }
                [JsonProperty("Self-DestructKills-MostinGame")]
                public string Self_DestructKills_MostinGame { get; set; }
                [JsonProperty("Multikill-Best")]
                public string Multikill_Best { get; set; }
                [JsonProperty("Self-DestructKills-Average")]
                public string Self_DestructKills_Average { get; set; }
            }
            public class Hero_Widowmaker
            {
                public string VenomMineKills { get; set; }
                public string ScopedHits { get; set; }
                public string ScopedShots { get; set; }
                public string ScopedCriticalHits { get; set; }
                [JsonProperty("ScopedCriticalHits-MostinGame")]
                public string ScopedCriticalHits_MostinGame { get; set; }
                [JsonProperty("VenomMineKills-MostinGame")]
                public string VenomMineKills_MostinGame { get; set; }
                [JsonProperty("ReconAssists-MostinGame")]
                public string ReconAssists_MostinGame { get; set; }
                [JsonProperty("ScopedAccuracy-BestinGame")]
                public string ScopedAccuracy_BestinGame { get; set; }
                [JsonProperty("MeleeFinalBlows-MostinGame")]
                public string MeleeFinalBlows_MostinGame { get; set; }
                [JsonProperty("VenomMineKills-Average")]
                public string VenomMineKills_Average { get; set; }
                [JsonProperty("ScopedCriticalHits-Average")]
                public string ScopedCriticalHits_Average { get; set; }
                public string ScopedAccuracy { get; set; }
                public string Eliminations { get; set; }
                public string FinalBlows { get; set; }
                public string SoloKills { get; set; }
                public string ShotsFired { get; set; }
                public string ShotsHit { get; set; }
                public string CriticalHits { get; set; }
                public string DamageDone { get; set; }
                public string ObjectiveKills { get; set; }
                public string Multikills { get; set; }
                public string MeleeFinalBlows { get; set; }
                public string CriticalHitsperMinute { get; set; }
                public string CriticalHitAccuracy { get; set; }
                public string EliminationsperLife { get; set; }
                public string WeaponAccuracy { get; set; }
                public string TeleporterPadsDestroyed { get; set; }
                public string TurretsDestroyed { get; set; }
                [JsonProperty("Eliminations-MostinLife")]
                public string Eliminations_MostinLife { get; set; }
                [JsonProperty("DamageDone-MostinLife")]
                public string DamageDone_MostinLife { get; set; }
                [JsonProperty("WeaponAccuracy-BestinGame")]
                public string WeaponAccuracy_BestinGame { get; set; }
                [JsonProperty("KillStreak-Best")]
                public string KillStreak_Best { get; set; }
                [JsonProperty("DamageDone-MostinGame")]
                public string DamageDone_MostinGame { get; set; }
                [JsonProperty("Eliminations-MostinGame")]
                public string Eliminations_MostinGame { get; set; }
                [JsonProperty("FinalBlows-MostinGame")]
                public string FinalBlows_MostinGame { get; set; }
                [JsonProperty("ObjectiveKills-MostinGame")]
                public string ObjectiveKills_MostinGame { get; set; }
                [JsonProperty("ObjectiveTime-MostinGame")]
                public string ObjectiveTime_MostinGame { get; set; }
                [JsonProperty("SoloKills-MostinGame")]
                public string SoloKills_MostinGame { get; set; }
                [JsonProperty("CriticalHits-MostinGame")]
                public string CriticalHits_MostinGame { get; set; }
                [JsonProperty("CriticalHits-MostinLife")]
                public string CriticalHits_MostinLife { get; set; }
                [JsonProperty("MeleeFinalBlows-Average")]
                public string MeleeFinalBlows_Average { get; set; }
                [JsonProperty("Deaths-Average")]
                public string Deaths_Average { get; set; }
                [JsonProperty("SoloKills-Average")]
                public string SoloKills_Average { get; set; }
                [JsonProperty("ObjectiveTime-Average")]
                public string ObjectiveTime_Average { get; set; }
                [JsonProperty("ObjectiveKills-Average")]
                public string ObjectiveKills_Average { get; set; }
                [JsonProperty("FinalBlows-Average")]
                public string FinalBlows_Average { get; set; }
                [JsonProperty("Eliminations-Average")]
                public string Eliminations_Average { get; set; }
                [JsonProperty("DamageDone-Average")]
                public string DamageDone_Average { get; set; }
                public string Deaths { get; set; }
                public string EnvironmentalDeaths { get; set; }
                [JsonProperty("Medals-Bronze")]
                public string Medals_Bronze { get; set; }
                [JsonProperty("Medals-Silver")]
                public string Medals_Silver { get; set; }
                [JsonProperty("Medals-Gold")]
                public string Medals_Gold { get; set; }
                public string Medals { get; set; }
                public string Cards { get; set; }
                public string TimePlayed { get; set; }
                public string GamesWon { get; set; }
                public string ObjectiveTime { get; set; }
                public string TimeSpentonFire { get; set; }
                [JsonProperty("Multikill-Best")]
                public string Multikill_Best { get; set; }
                [JsonProperty("ReconAssists-Average")]
                public string ReconAssists_Average { get; set; }
            }
            public class Hero_Tobjoern
            {
                public string ArmorPacksCreated { get; set; }
                [JsonProperty("Torbj&#xF6;rnKills-MostinGame")]
                public string TobjoernKills { get; set; }
                public string TurretKills { get; set; }
                [JsonProperty("Torbj&#xF6;rnKills-MostinGame")]
                public string TobjoernKills_MostinGame { get; set; }
                public string MoltenCoreKills { get; set; }
                [JsonProperty("MoltenCoreKills-MostinGame")]
                public string MoltenCoreKills_MostinGame { get; set; }
                [JsonProperty("MeleeFinalBlows-MostinGame")]
                public string MeleeFinalBlows_MostinGame { get; set; }
                [JsonProperty("TurretKills-Average")]
                public string TurretKills_Average { get; set; }
                [JsonProperty("Torbj&#xF6;rnKills-Average")]
                public string TobjoernKills_Average { get; set; }
                [JsonProperty("MoltenCoreKills-Average")]
                public string MoltenCoreKills_Average { get; set; }
                [JsonProperty("ArmorPacksCreated-Average")]
                public string ArmorPacksCreated_Average { get; set; }
                public string Eliminations { get; set; }
                public string FinalBlows { get; set; }
                public string SoloKills { get; set; }
                public string ShotsFired { get; set; }
                public string ShotsHit { get; set; }
                public string CriticalHits { get; set; }
                public string DamageDone { get; set; }
                public string ObjectiveKills { get; set; }
                public string Multikills { get; set; }
                public string MeleeFinalBlows { get; set; }
                public string CriticalHitsperMinute { get; set; }
                public string CriticalHitAccuracy { get; set; }
                public string EliminationsperLife { get; set; }
                public string WeaponAccuracy { get; set; }
                [JsonProperty("Eliminations-MostinLife")]
                public string Eliminations_MostinLife { get; set; }
                [JsonProperty("DamageDone-MostinLife")]
                public string DamageDone_MostinLife { get; set; }
                [JsonProperty("WeaponAccuracy-BestinGame")]
                public string WeaponAccuracy_BestinGame { get; set; }
                [JsonProperty("KillStreak-Best")]
                public string KillStreak_Best { get; set; }
                [JsonProperty("DamageDone-MostinGame")]
                public string DamageDone_MostinGame { get; set; }
                [JsonProperty("Eliminations-MostinGame")]
                public string Eliminations_MostinGame { get; set; }
                [JsonProperty("FinalBlows-MostinGame")]
                public string FinalBlows_MostinGame { get; set; }
                [JsonProperty("ObjectiveKills-MostinGame")]
                public string ObjectiveKills_MostinGame { get; set; }
                [JsonProperty("ObjectiveTime-MostinGame")]
                public string ObjectiveTime_MostinGame { get; set; }
                [JsonProperty("SoloKills-MostinGame")]
                public string SoloKills_MostinGame { get; set; }
                [JsonProperty("CriticalHits-MostinGame")]
                public string CriticalHits_MostinGame { get; set; }
                [JsonProperty("CriticalHits-MostinLife")]
                public string CriticalHits_MostinLife { get; set; }
                [JsonProperty("MeleeFinalBlows-Average")]
                public string MeleeFinalBlows_Average { get; set; }
                [JsonProperty("Deaths-Average")]
                public string Deaths_Average { get; set; }
                [JsonProperty("SoloKills-Average")]
                public string SoloKills_Average { get; set; }
                [JsonProperty("ObjectiveTime-Average")]
                public string ObjectiveTime_Average { get; set; }
                [JsonProperty("ObjectiveKills-Average")]
                public string ObjectiveKills_Average { get; set; }
                [JsonProperty("FinalBlows-Average")]
                public string FinalBlows_Average { get; set; }
                [JsonProperty("Eliminations-Average")]
                public string Eliminations_Average { get; set; }
                [JsonProperty("DamageDone-Average")]
                public string DamageDone_Average { get; set; }
                public string Deaths { get; set; }
                public string EnvironmentalDeath { get; set; }
                [JsonProperty("Medals-Bronze")]
                public string Medals_Bronze { get; set; }
                [JsonProperty("Medals-Silver")]
                public string Medals_Silver { get; set; }
                [JsonProperty("Medals-Gold")]
                public string Medals_Gold { get; set; }
                public string Medals { get; set; }
                public string Cards { get; set; }
                public string TimePlayed { get; set; }
                public string GamesWon { get; set; }
                public string ObjectiveTime { get; set; }
                public string TimeSpentonFire { get; set; }
                [JsonProperty("Multikill-Best")]
                public string Multikill_Best { get; set; }
            }
            public class Hero_Mei
            {
                public string EnemiesFrozen { get; set; }
                [JsonProperty("EnemiesFrozen-MostinGame")]
                public string EnemiesFrozen_MostinGame { get; set; }
                [JsonProperty("BlizzardKills-MostinGame")]
                public string BlizzardKills_MostinGame { get; set; }
                public string BlizzardKills { get; set; }
                [JsonProperty("DamageBlocked-MostinGame")]
                public string DamageBlocked_MostinGame { get; set; }
                public string DamageBlocked { get; set; }
                [JsonProperty("EnemiesFrozen-Average")]
                public string EnemiesFrozen_Average { get; set; }
                [JsonProperty("DamageBlocked-Average")]
                public string DamageBlocked_Average { get; set; }
                [JsonProperty("BlizzardKills-Average")]
                public string BlizzardKills_Average { get; set; }
                public string Eliminations { get; set; }
                public string FinalBlows { get; set; }
                public string SoloKills { get; set; }
                public string ShotsFired { get; set; }
                public string ShotsHit { get; set; }
                public string CriticalHits { get; set; }
                public string DamageDone { get; set; }
                public string ObjectiveKills { get; set; }
                public string CriticalHitsperMinute { get; set; }
                public string CriticalHitAccuracy { get; set; }
                public string EliminationsperLife { get; set; }
                public string WeaponAccuracy { get; set; }
                public string HealingDone { get; set; }
                public string TurretDestroyed { get; set; }
                public string SelfHealing { get; set; }
                [JsonProperty("Eliminations-MostinLife")]
                public string Eliminations_MostinLife { get; set; }
                [JsonProperty("DamageDone-MostinLife")]
                public string DamageDone_MostinLife { get; set; }
                [JsonProperty("HealingDone-MostinLife")]
                public string HealingDone_MostinLife { get; set; }
                [JsonProperty("WeaponAccuracy-BestinGame")]
                public string WeaponAccuracy_BestinGame { get; set; }
                [JsonProperty("KillStreak-Best")]
                public string KillStreak_Best { get; set; }
                [JsonProperty("DamageDone-MostinGame")]
                public string DamageDone_MostinGame { get; set; }
                [JsonProperty("HealingDone-MostinGame")]
                public string HealingDone_MostinGame { get; set; }
                [JsonProperty("Eliminations-MostinGame")]
                public string Eliminations_MostinGame { get; set; }
                [JsonProperty("FinalBlows-MostinGame")]
                public string FinalBlows_MostinGame { get; set; }
                [JsonProperty("ObjectiveKills-MostinGame")]
                public string ObjectiveKills_MostinGame { get; set; }
                [JsonProperty("ObjectiveTime-MostinGame")]
                public string ObjectiveTime_MostinGame { get; set; }
                [JsonProperty("SoloKills-MostinGame")]
                public string SoloKills_MostinGame { get; set; }
                [JsonProperty("CriticalHits-MostinGame")]
                public string CriticalHits_MostinGame { get; set; }
                [JsonProperty("CriticalHits-MostinLife")]
                public string CriticalHits_MostinLife { get; set; }
                [JsonProperty("SelfHealing-MostinGame")]
                public string SelfHealing_MostinGame { get; set; }
                [JsonProperty("SelfHealing-Average")]
                public string SelfHealing_Average { get; set; }
                [JsonProperty("Deaths-Average")]
                public string Deaths_Average { get; set; }
                [JsonProperty("SoloKills-Average")]
                public string SoloKills_Average { get; set; }
                [JsonProperty("ObjectiveTime-Average")]
                public string ObjectiveTime_Average { get; set; }
                [JsonProperty("ObjectiveKills-Average")]
                public string ObjectiveKills_Average { get; set; }
                [JsonProperty("HealingDone-Average")]
                public string HealingDone_Average { get; set; }
                [JsonProperty("FinalBlows-Average")]
                public string FinalBlows_Average { get; set; }
                [JsonProperty("Eliminations-Average")]
                public string Eliminations_Average { get; set; }
                [JsonProperty("DamageDone-Average")]
                public string DamageDone_Average { get; set; }
                public string Deaths { get; set; }
                [JsonProperty("Medals-Bronze")]
                public string Medals_Bronze { get; set; }
                [JsonProperty("Medals-Silver")]
                public string Medals_Silver { get; set; }
                [JsonProperty("Medals-Gold")]
                public string Medals_Gold { get; set; }
                public string Medals { get; set; }
                public string Card { get; set; }
                public string TimePlayed { get; set; }
                public string GamesWon { get; set; }
                public string ObjectiveTime { get; set; }
                public string TimeSpentonFire { get; set; }
            }
            public class Hero_Junkrat
            {
                [JsonProperty("EnemiesTrapped-MostinGame")]
                public string EnemiesTrapped_MostinGame { get; set; }
                public string EnemiesTrapped { get; set; }
                [JsonProperty("RIP-TireKills-MostinGame")]
                public string RIP_TireKills_MostinGame { get; set; }
                [JsonProperty("RIP-TireKills")]
                public string RIP_TireKills { get; set; }
                public string EnemiesTrappedaMinute { get; set; }
                public string Eliminations { get; set; }
                public string FinalBlows { get; set; }
                public string SoloKills { get; set; }
                public string ShotsFired { get; set; }
                public string ShotsHit { get; set; }
                public string DamageDone { get; set; }
                public string ObjectiveKills { get; set; }
                public string EliminationsperLife { get; set; }
                public string WeaponAccuracy { get; set; }
                [JsonProperty("Eliminations-MostinLife")]
                public string Eliminations_MostinLife { get; set; }
                [JsonProperty("DamageDone-MostinLife")]
                public string DamageDone_MostinLife { get; set; }
                [JsonProperty("WeaponAccuracy-BestinGame")]
                public string WeaponAccuracy_BestinGame { get; set; }
                [JsonProperty("KillStreak-Best")]
                public string KillStreak_Best { get; set; }
                [JsonProperty("DamageDone-MostinGame")]
                public string DamageDone_MostinGame { get; set; }
                [JsonProperty("Eliminations-MostinGame")]
                public string Eliminations_MostinGame { get; set; }
                [JsonProperty("FinalBlows-MostinGame")]
                public string FinalBlows_MostinGame { get; set; }
                [JsonProperty("ObjectiveKills-MostinGame")]
                public string ObjectiveKills_MostinGame { get; set; }
                [JsonProperty("ObjectiveTime-MostinGame")]
                public string ObjectiveTime_MostinGame { get; set; }
                [JsonProperty("SoloKills-MostinGame")]
                public string SoloKills_MostinGame { get; set; }
                [JsonProperty("Deaths-Average")]
                public string Deaths_Average { get; set; }
                [JsonProperty("SoloKills-Average")]
                public string SoloKills_Average { get; set; }
                [JsonProperty("ObjectiveTime-Average")]
                public string ObjectiveTime_Average { get; set; }
                [JsonProperty("ObjectiveKills-Average")]
                public string ObjectiveKills_Average { get; set; }
                [JsonProperty("FinalBlows-Average")]
                public string FinalBlows_Average { get; set; }
                [JsonProperty("Eliminations-Average")]
                public string Eliminations_Average { get; set; }
                [JsonProperty("DamageDone-Average")]
                public string DamageDone_Average { get; set; }
                public string Deaths { get; set; }
                [JsonProperty("Medals-Bronze")]
                public string Medals_Bronze { get; set; }
                [JsonProperty("Medals-Silver")]
                public string Medals_Silver { get; set; }
                [JsonProperty("Medals-Gold")]
                public string Medals_Gold { get; set; }
                public string Medals { get; set; }
                public string Card { get; set; }
                public string TimePlayed { get; set; }
                public string GamesWon { get; set; }
                public string ObjectiveTime { get; set; }
                [JsonProperty("RIP-TireKills-Average")]
                public string RIP_TireKills_Average { get; set; }
            }
            public class Hero_Hanzo
            {
                public string Elimination { get; set; }
                public string ShotsFired { get; set; }
                public string ShotsHit { get; set; }
                public string DamageDone { get; set; }
                public string ObjectiveKill { get; set; }
                public string EliminationperLife { get; set; }
                public string WeaponAccuracy { get; set; }
                [JsonProperty("Elimination-MostinLife")]
                public string Elimination_MostinLife { get; set; }
                [JsonProperty("DamageDone-MostinLife")]
                public string DamageDone_MostinLife { get; set; }
                [JsonProperty("KillStreak-Best")]
                public string KillStreak_Best { get; set; }
                [JsonProperty("DamageDone-MostinGame")]
                public string DamageDone_MostinGame { get; set; }
                [JsonProperty("Elimination-MostinGame")]
                public string Elimination_MostinGame { get; set; }
                [JsonProperty("ObjectiveKill-MostinGame")]
                public string ObjectiveKill_MostinGame { get; set; }
                [JsonProperty("ObjectiveTime-MostinGame")]
                public string ObjectiveTime_MostinGame { get; set; }
                [JsonProperty("Deaths-Average")]
                public string Deaths_Average { get; set; }
                [JsonProperty("ObjectiveTime-Average")]
                public string ObjectiveTime_Average { get; set; }
                [JsonProperty("ObjectiveKills-Average")]
                public string ObjectiveKills_Average { get; set; }
                [JsonProperty("Eliminations-Average")]
                public string Eliminations_Average { get; set; }
                [JsonProperty("DamageDone-Average")]
                public string DamageDone_Average { get; set; }
                public string Death { get; set; }
                [JsonProperty("Medals-Bronze")]
                public string Medals_Bronze { get; set; }
                [JsonProperty("Medals-Silver")]
                public string Medals_Silver { get; set; }
                public string Medals { get; set; }
                public string TimePlayed { get; set; }
                public string GamesWon { get; set; }
                public string ObjectiveTime { get; set; }
            }
            public class Hero_Bastion
            {
                public string ReconKills { get; set; }
                public string SentryKills { get; set; }
                public string TankKills { get; set; }
                [JsonProperty("SentryKills-MostinGame")]
                public string SentryKills_MostinGame { get; set; }
                [JsonProperty("ReconKills-MostinGame")]
                public string ReconKills_MostinGame { get; set; }
                [JsonProperty("TankKills-MostinGame")]
                public string TankKills_MostinGame { get; set; }
                [JsonProperty("MeleeFinalBlows-MostinGame")]
                public string MeleeFinalBlows_MostinGame { get; set; }
                [JsonProperty("TankKills-Average")]
                public string TankKills_Average { get; set; }
                [JsonProperty("SentryKills-Average")]
                public string SentryKills_Average { get; set; }
                [JsonProperty("ReconKills-Average")]
                public string ReconKills_Average { get; set; }
                public string Eliminations { get; set; }
                public string FinalBlows { get; set; }
                public string SoloKills { get; set; }
                public string ShotsFired { get; set; }
                public string ShotsHit { get; set; }
                public string CriticalHits { get; set; }
                public string DamageDone { get; set; }
                public string ObjectiveKills { get; set; }
                public string Multikills { get; set; }
                public string MeleeFinalBlows { get; set; }
                public string CriticalHitsperMinute { get; set; }
                public string CriticalHitAccuracy { get; set; }
                public string EliminationsperLife { get; set; }
                public string WeaponAccuracy { get; set; }
                public string HealingDone { get; set; }
                public string TurretsDestroyed { get; set; }
                public string SelfHealing { get; set; }
                [JsonProperty("Eliminations-Average")]
                public string Eliminations_MostinLife { get; set; }
                [JsonProperty("DamageDone-Average")]
                public string DamageDone_MostinLife { get; set; }
                [JsonProperty("HealingDone-MostinLife")]
                public string HealingDone_MostinLife { get; set; }
                [JsonProperty("WeaponAccuracy-BestinGame")]
                public string WeaponAccuracy_BestinGame { get; set; }
                [JsonProperty("KillStreak-Best")]
                public string KillStreak_Best { get; set; }
                [JsonProperty("DamageDone-MostinGame")]
                public string DamageDone_MostinGame { get; set; }
                [JsonProperty("HealingDone-MostinGame")]
                public string HealingDone_MostinGame { get; set; }
                [JsonProperty("Eliminations-MostinGame")]
                public string Eliminations_MostinGame { get; set; }
                [JsonProperty("FinalBlows-MostinGame")]
                public string FinalBlows_MostinGame { get; set; }
                [JsonProperty("ObjectiveKills-MostinGame")]
                public string ObjectiveKills_MostinGame { get; set; }
                [JsonProperty("ObjectiveTime-MostinGame")]
                public string ObjectiveTime_MostinGame { get; set; }
                [JsonProperty("SoloKills-MostinGame")]
                public string SoloKills_MostinGame { get; set; }
                [JsonProperty("CriticalHits-MostinGame")]
                public string CriticalHits_MostinGame { get; set; }
                [JsonProperty("CriticalHits-MostinLife")]
                public string CriticalHits_MostinLife { get; set; }
                [JsonProperty("SelfHealing-MostinGame")]
                public string SelfHealing_MostinGame { get; set; }
                [JsonProperty("MeleeFinalBlows-Average")]
                public string MeleeFinalBlows_Average { get; set; }
                [JsonProperty("SelfHealing-Average")]
                public string SelfHealing_Average { get; set; }
                [JsonProperty("Deaths-Average")]
                public string Deaths_Average { get; set; }
                [JsonProperty("SoloKills-Average")]
                public string SoloKills_Average { get; set; }
                [JsonProperty("ObjectiveTime-Average")]
                public string ObjectiveTime_Average { get; set; }
                [JsonProperty("ObjectiveKills-Average")]
                public string ObjectiveKills_Average { get; set; }
                [JsonProperty("HealingDone-Average")]
                public string HealingDone_Average { get; set; }
                [JsonProperty("FinalBlows-Average")]
                public string FinalBlows_Average { get; set; }
                [JsonProperty("Eliminations-Average")]
                public string Eliminations_Average { get; set; }
                [JsonProperty("DamageDone-Average")]
                public string DamageDone_Average { get; set; }
                public string Deaths { get; set; }
                public string EnvironmentalDeath { get; set; }
                [JsonProperty("Medals-Bronze")]
                public string Medals_Bronze { get; set; }
                [JsonProperty("Medals-Silver")]
                public string Medals_Silver { get; set; }
                [JsonProperty("Medals-Gold")]
                public string Medals_Gold { get; set; }
                public string Medals { get; set; }
                public string Cards { get; set; }
                public string TimePlayed { get; set; }
                public string GamesWon { get; set; }
                public string ObjectiveTime { get; set; }
                public string TimeSpentonFire { get; set; }
                [JsonProperty("Multikill-Best")]
                public string Multikill_Best { get; set; }
            }
            public class Hero_Tracer
            {
                public string PulseBombKill { get; set; }
                [JsonProperty("PulseBombKill-MostinGame")]
                public string PulseBombKill_MostinGame { get; set; }
                [JsonProperty("PulseBombsAttached-MostinGame")]
                public string PulseBombsAttached_MostinGame { get; set; }
                public string PulseBombsAttached { get; set; }
                [JsonProperty("MeleeFinalBlow-MostinGame")]
                public string MeleeFinalBlow_MostinGame { get; set; }
                [JsonProperty("PulseBombKills-Average")]
                public string PulseBombKills_Average { get; set; }
                [JsonProperty("PulseBombsAttached-Average")]
                public string PulseBombsAttached_Average { get; set; }
                public string Eliminations { get; set; }
                public string FinalBlows { get; set; }
                public string SoloKills { get; set; }
                public string ShotsFired { get; set; }
                public string ShotsHit { get; set; }
                public string CriticalHits { get; set; }
                public string DamageDone { get; set; }
                public string ObjectiveKills { get; set; }
                public string MeleeFinalBlow { get; set; }
                public string CriticalHitsperMinute { get; set; }
                public string CriticalHitAccuracy { get; set; }
                public string EliminationsperLife { get; set; }
                public string WeaponAccuracy { get; set; }
                public string SelfHealing { get; set; }
                [JsonProperty("Eliminations-MostinLife")]
                public string Eliminations_MostinLife { get; set; }
                [JsonProperty("DamageDone-MostinLife")]
                public string DamageDone_MostinLife { get; set; }
                [JsonProperty("WeaponAccuracy-BestinGam")]
                public string WeaponAccuracy_BestinGame { get; set; }
                [JsonProperty("KillStreak-est")]
                public string KillStreak_est { get; set; }
                [JsonProperty("DamageDone-MostinGame")]
                public string DamageDone_MostinGame { get; set; }
                [JsonProperty("Eliminations-MostinGame")]
                public string Eliminations_MostinGame { get; set; }
                [JsonProperty("FinalBlows-MostinGame")]
                public string FinalBlows_MostinGame { get; set; }
                [JsonProperty("ObjectiveKills-MostinGame")]
                public string ObjectiveKills_MostinGame { get; set; }
                [JsonProperty("ObjectiveTime-MostinGame")]
                public string ObjectiveTime_MostinGame { get; set; }
                [JsonProperty("SoloKills-MostinGame")]
                public string SoloKills_MostinGame { get; set; }
                [JsonProperty("CriticalHits-MostinGame")]
                public string CriticalHits_MostinGame { get; set; }
                [JsonProperty("CriticalHits-MostinLife")]
                public string CriticalHits_MostinLife { get; set; }
                [JsonProperty("SelfHealing-MostinGame")]
                public string SelfHealing_MostinGame { get; set; }
                [JsonProperty("MeleeFinalBlows-Average")]
                public string MeleeFinalBlows_Average { get; set; }
                [JsonProperty("SelfHealing-Average")]
                public string SelfHealing_Average { get; set; }
                [JsonProperty("Deaths-Average")]
                public string Deaths_Average { get; set; }
                [JsonProperty("SoloKills-Average")]
                public string SoloKills_Average { get; set; }
                [JsonProperty("ObjectiveTime-Average")]
                public string ObjectiveTime_Average { get; set; }
                [JsonProperty("ObjectiveKills-Average")]
                public string ObjectiveKills_Average { get; set; }
                [JsonProperty("FinalBlows-Average")]
                public string FinalBlows_Average { get; set; }
                [JsonProperty("Eliminations-Average")]
                public string Eliminations_Average { get; set; }
                [JsonProperty("DamageDone-Average")]
                public string DamageDone_Average { get; set; }
                public string Deaths { get; set; }
                [JsonProperty("Medals-Bronze")]
                public string Medals_Bronze { get; set; }
                [JsonProperty("Medals-Silver")]
                public string Medals_Silver { get; set; }
                [JsonProperty("Medals-Gold")]
                public string Medals_Gold { get; set; }
                public string Medals { get; set; }
                public string TimePlayed { get; set; }
                public string GamesPlayed { get; set; }
                public string GamesWon { get; set; }
                public string ObjectiveTime { get; set; }
                public string TimeSpentonFire { get; set; }
                public string WinPercentage { get; set; }
                public string GamesTied { get; set; }
                public string GamesLost { get; set; }
            }
            public class Hero_Soldier76
            {
                [JsonProperty("HelixRocketsKills-MostinGame")]
                public string HelixRocketsKills_MostinGame { get; set; }
                public string HelixRocketsKills { get; set; }
                public string TacticalVisorKills { get; set; }
                [JsonProperty("TacticalVisorKills-MostinGame")]
                public string TacticalVisorKills_MostinGame { get; set; }
                public string BioticFieldsDeployed { get; set; }
                public string BioticFieldHealingDone { get; set; }
                [JsonProperty("MeleeFinalBlow-MostinGame")]
                public string MeleeFinalBlow_MostinGame { get; set; }
                [JsonProperty("TacticalVisorKills-Average")]
                public string TacticalVisorKills_Average { get; set; }
                [JsonProperty("HelixRocketsKills-Average")]
                public string HelixRocketsKills_Average { get; set; }
                public string Eliminations { get; set; }
                public string FinalBlows { get; set; }
                public string SoloKills { get; set; }
                public string ShotsFired { get; set; }
                public string ShotsHit { get; set; }
                public string CriticalHits { get; set; }
                public string DamageDone { get; set; }
                public string ObjectiveKills { get; set; }
                public string Multikill { get; set; }
                public string MeleeFinalBlow { get; set; }
                public string CriticalHitsperMinute { get; set; }
                public string CriticalHitAccuracy { get; set; }
                public string EliminationsperLife { get; set; }
                public string WeaponAccuracy { get; set; }
                public string HealingDone { get; set; }
                public string TurretsDestroyed { get; set; }
                public string SelfHealing { get; set; }
                [JsonProperty("Eliminations-MostinLife")]
                public string Eliminations_MostinLife { get; set; }
                [JsonProperty("DamageDone-MostinLife")]
                public string DamageDone_MostinLife { get; set; }
                [JsonProperty("HealingDone-MostinLife")]
                public string HealingDone_MostinLife { get; set; }
                [JsonProperty("WeaponAccuracy-BestinGame")]
                public string WeaponAccuracy_BestinGame { get; set; }
                [JsonProperty("KillStreak-Best")]
                public string KillStreak_Best { get; set; }
                [JsonProperty("DamageDone-MostinGame")]
                public string DamageDone_MostinGame { get; set; }
                [JsonProperty("HealingDone-MostinGame")]
                public string HealingDone_MostinGame { get; set; }
                [JsonProperty("Eliminations-MostinGame")]
                public string Eliminations_MostinGame { get; set; }
                [JsonProperty("FinalBlows-MostinGame")]
                public string FinalBlows_MostinGame { get; set; }
                [JsonProperty("ObjectiveKills-MostinGame")]
                public string ObjectiveKills_MostinGame { get; set; }
                [JsonProperty("ObjectiveTime-MostinGame")]
                public string ObjectiveTime_MostinGame { get; set; }
                [JsonProperty("SoloKills-MostinGame")]
                public string SoloKills_MostinGame { get; set; }
                [JsonProperty("CriticalHits-MostinGame")]
                public string CriticalHits_MostinGame { get; set; }
                [JsonProperty("CriticalHits-MostinLife")]
                public string CriticalHits_MostinLife { get; set; }
                [JsonProperty("SelfHealing-MostinGame")]
                public string SelfHealing_MostinGame { get; set; }
                [JsonProperty("MeleeFinalBlows-Average")]
                public string MeleeFinalBlows_Average { get; set; }
                [JsonProperty("SelfHealing-Average")]
                public string SelfHealing_Average { get; set; }
                [JsonProperty("Deaths-Average")]
                public string Deaths_Average { get; set; }
                [JsonProperty("SoloKills-Average")]
                public string SoloKills_Average { get; set; }
                [JsonProperty("ObjectiveTime-Average")]
                public string ObjectiveTime_Average { get; set; }
                [JsonProperty("ObjectiveKills-Average")]
                public string ObjectiveKills_Average { get; set; }
                [JsonProperty("HealingDone-Average")]
                public string HealingDone_Average { get; set; }
                [JsonProperty("FinalBlows-Average")]
                public string FinalBlows_Average { get; set; }
                [JsonProperty("Eliminations-Average")]
                public string Eliminations_Average { get; set; }
                [JsonProperty("DamageDone-Average")]
                public string DamageDone_Average { get; set; }
                public string Deaths { get; set; }
                [JsonProperty("Medals-Bronze")]
                public string Medals_Bronze { get; set; }
                [JsonProperty("Medals-Silver")]
                public string Medals_Silver { get; set; }
                [JsonProperty("Medals-Gold")]
                public string Medals_Gold { get; set; }
                public string Medals { get; set; }
                public string Card { get; set; }
                public string TimePlayed { get; set; }
                public string GamesPlayed { get; set; }
                public string GamesWon { get; set; }
                public string ObjectiveTime { get; set; }
                public string TimeSpentonFire { get; set; }
                public string WinPercentage { get; set; }
                [JsonProperty("Multikill-Best")]
                public string Multikill_Best { get; set; }
                public string GamesTied { get; set; }
                public string GamesLost { get; set; }
            }
            public class Hero_Reaper
            {
                public string SoulConsumed { get; set; }
                [JsonProperty("SoulConsumed-MostinGame")]
                public string SoulConsumed_MostinGame { get; set; }
                [JsonProperty("SoulConsumed-Average")]
                public string SoulsConsumed_Average { get; set; }
                public string ShotsFired { get; set; }
                public string ShotsHit { get; set; }
                public string CriticalHits { get; set; }
                public string DamageDone { get; set; }
                public string CriticalHitsperMinute { get; set; }
                public string CriticalHitAccuracy { get; set; }
                public string WeaponAccuracy { get; set; }
                public string HealingDone { get; set; }
                public string SelfHealing { get; set; }
                [JsonProperty("DamageDone-MostinLife")]
                public string DamageDone_MostinLife { get; set; }
                [JsonProperty("HealingDone-MostinLife")]
                public string HealingDone_MostinLife { get; set; }
                [JsonProperty("DamageDone-MostinGame")]
                public string DamageDone_MostinGame { get; set; }
                [JsonProperty("HealingDone-MostinGame")]
                public string HealingDone_MostinGame { get; set; }
                [JsonProperty("ObjectiveTime-MostinGame")]
                public string ObjectiveTime_MostinGame { get; set; }
                [JsonProperty("CriticalHits-MostinGame")]
                public string CriticalHits_MostinGame { get; set; }
                [JsonProperty("CriticalHits-MostinLife")]
                public string CriticalHits_MostinLife { get; set; }
                [JsonProperty("SelfHealing-MostinGame")]
                public string SelfHealing_MostinGame { get; set; }
                [JsonProperty("SelfHealing-Average")]
                public string SelfHealing_Average { get; set; }
                [JsonProperty("Deaths-Average")]
                public string Deaths_Average { get; set; }
                [JsonProperty("ObjectiveTime-Average")]
                public string ObjectiveTime_Average { get; set; }
                [JsonProperty("HealingDone-Average")]
                public string HealingDone_Average { get; set; }
                [JsonProperty("DamageDone-Average")]
                public string DamageDone_Average { get; set; }
                public string Deaths { get; set; }
                [JsonProperty("Medals-Bronze")]
                public string Medals_Bronze { get; set; }
                [JsonProperty("Medals-Gold")]
                public string Medals_Gold { get; set; }
                public string Medals { get; set; }
                public string TimePlayed { get; set; }
                public string GamesPlayed { get; set; }
                public string ObjectiveTime { get; set; }
                public string GamesTied { get; set; }
                public string GamesLost { get; set; }
            }
            public class Hero_Pharah
            {
                public string RocketDirectHits { get; set; }
                public string BarrageKills { get; set; }
                [JsonProperty("RocketDirectHits-MostinGame")]
                public string RocketDirectHits_MostinGame { get; set; }
                [JsonProperty("BarrageKills-MostinGame")]
                public string BarrageKills_MostinGame { get; set; }
                [JsonProperty("RocketDirectHits-Average")]
                public string RocketDirectHits_Average { get; set; }
                [JsonProperty("BarrageKills-Average")]
                public string BarrageKills_Average { get; set; }
                public string Eliminations { get; set; }
                public string FinalBlows { get; set; }
                public string SoloKills { get; set; }
                public string ShotsFired { get; set; }
                public string ShotsHit { get; set; }
                public string DamageDone { get; set; }
                public string ObjectiveKills { get; set; }
                public string Multikills { get; set; }
                public string EliminationsperLife { get; set; }
                public string WeaponAccuracy { get; set; }
                public string TeleporterPadDestroyed { get; set; }
                [JsonProperty("Eliminations-MostinLife")]
                public string Eliminations_MostinLife { get; set; }
                [JsonProperty("DamageDone-MostinLife")]
                public string DamageDone_MostinLife { get; set; }
                [JsonProperty("WeaponAccuracy-BestinGame")]
                public string WeaponAccuracy_BestinGame { get; set; }
                [JsonProperty("KillStreak-Best")]
                public string KillStreak_Best { get; set; }
                [JsonProperty("DamageDone-MostinGame")]
                public string DamageDone_MostinGame { get; set; }
                [JsonProperty("Eliminations-MostinGame")]
                public string Eliminations_MostinGame { get; set; }
                [JsonProperty("FinalBlows-MostinGame")]
                public string FinalBlows_MostinGame { get; set; }
                [JsonProperty("ObjectiveKills-MostinGame")]
                public string ObjectiveKills_MostinGame { get; set; }
                [JsonProperty("ObjectiveTime-MostinGame")]
                public string ObjectiveTime_MostinGame { get; set; }
                [JsonProperty("SoloKills-MostinGame")]
                public string SoloKills_MostinGame { get; set; }
                [JsonProperty("Deaths-Average")]
                public string Deaths_Average { get; set; }
                [JsonProperty("SoloKills-Average")]
                public string SoloKills_Average { get; set; }
                [JsonProperty("ObjectiveTime-Average")]
                public string ObjectiveTime_Average { get; set; }
                [JsonProperty("ObjectiveKills-Average")]
                public string ObjectiveKills_Average { get; set; }
                [JsonProperty("FinalBlows-Average")]
                public string FinalBlows_Average { get; set; }
                [JsonProperty("Eliminations-Average")]
                public string Eliminations_Average { get; set; }
                [JsonProperty("DamageDone-Average")]
                public string DamageDone_Average { get; set; }
                public string Deaths { get; set; }
                [JsonProperty("Medals-Bronze")]
                public string Medals_Bronze { get; set; }
                [JsonProperty("Medals-Silver")]
                public string Medals_Silver { get; set; }
                [JsonProperty("Medals-Gold")]
                public string Medals_Gold { get; set; }
                public string Medals { get; set; }
                public string TimePlayed { get; set; }
                public string GamesPlayed { get; set; }
                public string GamesWon { get; set; }
                public string ObjectiveTime { get; set; }
                public string TimeSpentonFire { get; set; }
                public string WinPercentage { get; set; }
                [JsonProperty("Multikill-Best")]
                public string Multikill_Best { get; set; }
                public string GamesLost { get; set; }
            }
            public class Hero_Mccree
            {
                public string DeadeyeKills { get; set; }
                [JsonProperty("DeadeyeKills-MostinGame")]
                public string DeadeyeKills_MostinGame { get; set; }
                public string FantheHammerKills { get; set; }
                [JsonProperty("FantheHammerKills-Average")]
                public string FantheHammerKills_Average { get; set; }
                [JsonProperty("DeadeyeKills-Average")]
                public string DeadeyeKills_Average { get; set; }
                public string Eliminations { get; set; }
                public string FinalBlows { get; set; }
                public string ShotsFired { get; set; }
                public string ShotsHit { get; set; }
                public string CriticalHits { get; set; }
                public string DamageDone { get; set; }
                public string ObjectiveKills { get; set; }
                public string CriticalHitsperMinute { get; set; }
                public string CriticalHitAccuracy { get; set; }
                public string EliminationsperLife { get; set; }
                public string WeaponAccuracy { get; set; }
                [JsonProperty("Eliminations-MostinLife")]
                public string Eliminations_MostinLife { get; set; }
                [JsonProperty("DamageDone-MostinLife")]
                public string DamageDone_MostinLife { get; set; }
                [JsonProperty("KillStreak_Best")]
                public string KillStreak_Best { get; set; }
                [JsonProperty("DamageDone-MostinGame")]
                public string DamageDone_MostinGame { get; set; }
                [JsonProperty("Eliminations-MostinGame")]
                public string Eliminations_MostinGame { get; set; }
                [JsonProperty("FinalBlows-MostinGame")]
                public string FinalBlows_MostinGame { get; set; }
                [JsonProperty("ObjectiveKills-MostinGame")]
                public string ObjectiveKills_MostinGame { get; set; }
                [JsonProperty("ObjectiveTime-MostinGame")]
                public string ObjectiveTime_MostinGame { get; set; }
                [JsonProperty("CriticalHits-MostinGame")]
                public string CriticalHits_MostinGame { get; set; }
                [JsonProperty("CriticalHit-MostinLife")]
                public string CriticalHit_MostinLife { get; set; }
                [JsonProperty("Deaths-Average")]
                public string Deaths_Average { get; set; }
                [JsonProperty("ObjectiveTime-Average")]
                public string ObjectiveTime_Average { get; set; }
                [JsonProperty("ObjectiveKills-Average")]
                public string ObjectiveKills_Average { get; set; }
                [JsonProperty("FinalBlows-Average")]
                public string FinalBlows_Average { get; set; }
                [JsonProperty("Eliminations-Average")]
                public string Eliminations_Average { get; set; }
                [JsonProperty("DamageDone-Average")]
                public string DamageDone_Average { get; set; }
                public string Deaths { get; set; }
                [JsonProperty("Medals-Bronze")]
                public string Medals_Bronze { get; set; }
                [JsonProperty("Medals-Silver")]
                public string Medals_Silver { get; set; }
                [JsonProperty("Medals-Gold")]
                public string Medals_Gold { get; set; }
                public string Medals { get; set; }
                public string TimePlayed { get; set; }
                public string GamesPlayed { get; set; }
                public string GamesWon { get; set; }
                public string ObjectiveTime { get; set; }
                public string TimeSpentonFire { get; set; }
                public string WinPercentage { get; set; }
                [JsonProperty("FantheHammerKills-MostinGame")]
                public string FantheHammerKills_MostinGame { get; set; }
                public string GamesTied { get; set; }
                public string GamesLost { get; set; }
            }
            public class Hero_Genji
            {
                public string DragonbladeKills { get; set; }
                [JsonProperty("DragonbladeKills-MostinGame")]
                public string DragonbladeKills_MostinGame { get; set; }
                public string DamageReflected { get; set; }
                [JsonProperty("DamageReflected-MostinGame")]
                public string DamageReflected_MostinGame { get; set; }
                public string Dragonblades { get; set; }
                [JsonProperty("MeleeFinalBlow-MostinGame")]
                public string MeleeFinalBlow_MostinGame { get; set; }
                [JsonProperty("DragonbladeKills-Average")]
                public string DragonbladeKills_Average { get; set; }
                [JsonProperty("DamageReflected-Average")]
                public string DamageReflected_Average { get; set; }
                public string Eliminations { get; set; }
                public string FinalBlows { get; set; }
                public string SoloKills { get; set; }
                public string ShotsFired { get; set; }
                public string ShotsHit { get; set; }
                public string CriticalHits { get; set; }
                public string DamageDone { get; set; }
                public string ObjectiveKills { get; set; }
                public string MeleeFinalBlow { get; set; }
                public string CriticalHitsperMinute { get; set; }
                public string CriticalHitAccuracy { get; set; }
                public string EliminationsperLife { get; set; }
                public string WeaponAccuracy { get; set; }
                [JsonProperty("Eliminations-MostinLife")]
                public string Eliminations_MostinLife { get; set; }
                [JsonProperty("DamageDone-MostinLife")]
                public string DamageDone_MostinLife { get; set; }
                [JsonProperty("KillStreak_Best")]
                public string KillStreak_Best { get; set; }
                [JsonProperty("DamageDone-MostinGame")]
                public string DamageDone_MostinGame { get; set; }
                [JsonProperty("Eliminations-MostinGame")]
                public string Eliminations_MostinGame { get; set; }
                [JsonProperty("FinalBlows-MostinGame")]
                public string FinalBlows_MostinGame { get; set; }
                [JsonProperty("ObjectiveKills-MostinGame")]
                public string ObjectiveKills_MostinGame { get; set; }
                [JsonProperty("ObjectiveTime-MostinGame")]
                public string ObjectiveTime_MostinGame { get; set; }
                [JsonProperty("SoloKills-MostinGame")]
                public string SoloKills_MostinGame { get; set; }
                [JsonProperty("CriticalHits-MostinGame")]
                public string CriticalHits_MostinGame { get; set; }
                [JsonProperty("CriticalHits-MostinLife")]
                public string CriticalHits_MostinLife { get; set; }
                [JsonProperty("MeleeFinalBlows-Average")]
                public string MeleeFinalBlows_Average { get; set; }
                [JsonProperty("Deaths-Average")]
                public string Deaths_Average { get; set; }
                [JsonProperty("SoloKills-Average")]
                public string SoloKills_Average { get; set; }
                [JsonProperty("ObjectiveTime-Average")]
                public string ObjectiveTime_Average { get; set; }
                [JsonProperty("ObjectiveKills-Average")]
                public string ObjectiveKills_Average { get; set; }
                [JsonProperty("FinalBlows-Average")]
                public string FinalBlows_Average { get; set; }
                [JsonProperty("Eliminations-Average")]
                public string Eliminations_Average { get; set; }
                [JsonProperty("DamageDone-Average")]
                public string DamageDone_Average { get; set; }
                public string Deaths { get; set; }
                [JsonProperty("Medals-Bronze")]
                public string Medals_Bronze { get; set; }
                [JsonProperty("Medals-Silver")]
                public string Medals_Silver { get; set; }
                [JsonProperty("Medals-Gold")]
                public string Medals_Gold { get; set; }
                public string Medals { get; set; }
                public string TimePlayed { get; set; }
                public string GamesPlayed { get; set; }
                public string GamesWon { get; set; }
                public string ObjectiveTime { get; set; }
                public string TimeSpentonFire { get; set; }
                public string WinPercentage { get; set; }
                public string GamesTied { get; set; }
                public string GamesLost { get; set; }
            }
        }

        public class OverwatchAllHeroes
        {
            public string MeleeFinalBlows { get; set; }
            public string SoloKills { get; set; }
            public string ObjectiveKills { get; set; }
            public string FinalBlows { get; set; }
            public string DamageDone { get; set; }
            public string Eliminations { get; set; }
            public string Multikills { get; set; }
            public string ReconAssists { get; set; }
            public string HealingDone { get; set; }
            public string TeleporterPadDestroyed { get; set; }
            public string Eliminations_MostinGame { get; set; }
            public string FinalBlows_MostinGame { get; set; }
            public string DamageDone_MostinGame { get; set; }
            public string HealingDone_MostinGame { get; set; }
            public string DefensiveAssists_MostinGame { get; set; }
            public string OffensiveAssists_MostinGame { get; set; }
            public string ObjectiveKills_MostinGame { get; set; }
            public string ObjectiveTime_MostinGame { get; set; }
            public string Multikill_Best { get; set; }
            public string SoloKills_MostinGame { get; set; }
            public string TimeSpentonFire_MostinGame { get; set; }
            public string MeleeFinalBlows_Average { get; set; }
            public string TimeSpentonFire_Average { get; set; }
            public string SoloKills_Average { get; set; }
            public string ObjectiveTime_Average { get; set; }
            public string ObjectiveKills_Average { get; set; }
            public string HealingDone_Average { get; set; }
            public string FinalBlows_Average { get; set; }
            public string Deaths_Average { get; set; }
            public string DamageDone_Average { get; set; }
            public string Eliminations_Average { get; set; }
            public string Deaths { get; set; }
            public string EnvironmentalDeaths { get; set; }
            public string Cards { get; set; }
            public string Medals { get; set; }
            public string Medals_Gold { get; set; }
            public string Medals_Silver { get; set; }
            public string Medals_Bronze { get; set; }
            public string GamesPlayed { get; set; }
            public string GamesWon { get; set; }
            public string TimeSpentonFire { get; set; }
            public string ObjectiveTime { get; set; }
            public string TimePlayed { get; set; }
            public string MeleeFinalBlows_MostinGame { get; set; }
            public string GamesTied { get; set; }
            public string GamesLost { get; set; }
            public string ReconAssists_Average { get; set; }
            public string DefensiveAssists { get; set; }
            public string DefensiveAssists_Average { get; set; }
            public string OffensiveAssists { get; set; }
            public string OffensiveAssists_Average { get; set; }
        }
        public class OverwatchPlayer
        {
            public bool Missing { get; set; } = false;
            public string username { get; set; }
            public int level { get; set; }
            public string avatar { get; set; }
            public string levelFrame { get; set; }
            public string star { get; set; }
            public OverwatchGames Games { get; set; }
            public OverwatchPlaytime Playtime { get; set; }
            public OverwatchCompetitive Competitive { get; set; }
            public class OverwatchGames
            {
                public OverwatchQG Quick { get; set; }
                public OverwatchCOMP Competitive { get; set; }

                public class OverwatchQG
                {
                    public string wins { get; set; }
                }
                public class OverwatchCOMP
                {
                    public string wins { get; set; }
                    public int lost { get; set; }
                    public string played { get; set; }
                }
            }
            public class OverwatchCompetitive
            {
                public string rank { get; set; }
                public string rank_img { get; set; }
            }
            public class OverwatchPlaytime
            {
                public string quick { get; set; }
                public string competitive { get; set; }
            }
        }

        public class OverwatchPatchNotes
        {
            public bool Missing { get; set; } = false;
            public string program { get; set; }
            public string locale { get; set; }
            public string type { get; set; }
            public string patchVersion { get; set; }
            public string status { get; set; }
            public string detail { get; set; }
            public float buildNumber { get; set; }
            public float publish { get; set; }
            public float created { get; set; }
            public bool updated { get; set; }
            public string slug { get; set; }
            public string version { get; set; }
        }

        public class OverwatchPagination
        {
            public bool Missing { get; set; } = false;
            public float totalEntries { get; set; }
            public float totalPages { get; set; }
            public float pageSize { get; set; }
            public float page { get; set; }
        }

        public class OverwatchAchievements
        {
            public bool Missing { get; set; } = false;
            public string name { get; set; }
            public bool finished { get; set; }
            public string image { get; set; }
            public string description { get; set; }
        }

        //This is to strip the html from patch notes content
        internal static string StripHTML(string input)
        {
            var re = Regex.Replace(input, "<.*?>", String.Empty);
            re = Regex.Replace(re, "&#160;", $@" ");
            return re;
        }
    }
}
