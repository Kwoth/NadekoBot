using Discord;
using Discord.Commands;
using NadekoBot.DataModels;
using NadekoBot.JSONModels;
using NadekoBot.Modules.Pokemon.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NadekoBot.Modules.Pokemon
{
    class PokemonChallenge
    {
        private Server server { get; }
        private Channel channel { get; }
        private User user { get; }
        public bool isWild { get; private set; }

        private CancellationTokenSource challengeCancelSource { get; set; }
        public bool ShouldStopChallenge { get; set; }
        private Random rng { get; }

        private PokemonSprite wildPokemon { get; set; }
        private PokemonSpecies wildSpecies { get; set; }

        public PokemonChallenge(CommandEventArgs e)
        {
            server = e.Server;
            channel = e.Channel;
            user = e.User;
            rng = new Random();
            challengeCancelSource = new CancellationTokenSource();
            var token = challengeCancelSource.Token;

            //Either it's a wild pokemon or a challenger!
            isWild = rng.Next(0, 100) > 50;
            if (isWild)
            {
                Task.Run(StartWildPokemonBattle);
            }
            else
            {
                Task.Run(StartChallenge);
            }

        }

        #region wildPokemonBattle

        private async Task StartWildPokemonBattle()
        {
            wildPokemon = PokemonModule.GeneratePokemon(NadekoBot.Client.CurrentUser.Id);
            wildSpecies = wildPokemon.GetSpecies();
            await IntroductionMessageWild();
             while (!ShouldStopChallenge)
            {
                //refresh target
                PokemonSprite userPokemon = PokemonModule.ActivePokemon(user);
                //Wild pokemon are faster :P
                var move = randomMove(wildSpecies);
                PokemonAttack attack = new PokemonAttack(wildPokemon, userPokemon, move);
                await channel.SendMessage(attack.AttackString());
            }
        }

        private KeyValuePair<string, string> randomMove(PokemonSpecies species)
        {
            
            return species.moves.ToList()[rng.Next(0, species.moves.Count - 1)];
        }

        private async Task<Message> IntroductionMessageWild()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{user.Name} is wandering around in the glorious world of Pokemon");
            sb.AppendLine($"Whoops! {user.Name} forgot about the dangers of the wild grass!");
            sb.AppendLine($"A wild Pokemon appeared! It's a {wildPokemon.NickName}!");


            return await channel.SendMessage(sb.ToString());
        }
        #endregion
        #region trainerbattle

        private async Task StartChallenge()
        {


            await IntroductionMessageChallenger();

            while (!ShouldStopChallenge)
            {
                //Nadeko is so much quicker!
                await channel.SendMessage($"");

            }

        }

        private async Task<Message> IntroductionMessageChallenger()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{user.Name} is wandering around in the glorious world of Pokemon");
            sb.AppendLine($"Suddenly, {user.Name} saw something dangerous in the distance..");
            sb.AppendLine($"{user.Name} ran towards the danger, drawing his pokemon from his bag!");
            sb.AppendLine($"Shock! {user.Name} realised that the danger was...!");

            return await channel.SendMessage(sb.ToString());
        }

        #endregion
    }
}
