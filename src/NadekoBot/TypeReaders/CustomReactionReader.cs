using Discord;
using Discord.Commands;
using NadekoBot.Services.Database.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NadekoBot.TypeReaders
{
    /// <summary>
    /// Reads in the setup of a *new* custom reaction
    /// This way uses regex, recommend improvement
    /// </summary>
    public class CustomReactionReader : TypeReader
    {
        private const string SERVER_PREFIX = "server ";

        public override Task<TypeReaderResult> Read(IUserMessage context, string input)
        {
            var react = new CustomReaction();
            if (input.StartsWith(SERVER_PREFIX) || !NadekoBot.Credentials.IsOwner(context.Author))
            {
                if (input.StartsWith(SERVER_PREFIX))
                    input = input.Substring(SERVER_PREFIX.Length);
                react.ServerId = (long)(context.Channel as IGuildChannel).Id;
            }
            input = input.TrimStart();
            string reactionName;
            switch (input.FirstOrDefault())
            {
                case '"':
                    reactionName = ParseContent(ref input, '"');
                    react.IsRegex = false;
                    break;

                case '/':
                    reactionName = ParseContent(ref input, '/');
                    try
                    {
                        react.Regex = new Regex(reactionName);
                        react.IsRegex = true;
                    }
                    catch (Exception e)
                    {
                        return Task.FromResult( TypeReaderResult.FromError(CommandError.ParseFailed, $"Regex parse failed: {e.Message}"));
                    }
                    break;

                case default(char):
                    return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "too short"));

                default:
                    reactionName = ParseContent(ref input, ' ');
                    react.IsRegex = false;
                    break;
            }

            if (reactionName == null)
            {
                return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Could not parse trigger"));
            }
            react.Trigger = reactionName;
            if (string.IsNullOrWhiteSpace(input))
            {
                return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "too short"));
            }
            react.Responses = new List<Response>(1);
            react.Responses.Add(new Response { Text = input.Trim() });
            return Task.FromResult(TypeReaderResult.FromSuccess(react));
        }

        /// <summary>
        /// Parses the string efficiently for the next occurence of the split character, ignoring escaped versions of the character 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="splitChar"></param>
        /// <returns></returns>
        private string ParseContent(ref string input, char splitChar)
        {
            StringBuilder sb = new StringBuilder();
            using (var reader = new StringReader(input))
            {
                char next;
                char current;
                if ((next = (char)reader.Read()) != splitChar)
                    sb.Append(next);
                while ((current = (char)reader.Read()) != default(char))
                {
                    next = (char)reader.Peek();
                    if (next == splitChar)
                    {
                        if (current == '\\') //allowing escaping :D
                            //do we want this for spaces?
                        {
                            sb.Append(next);
                            reader.Read();
                        }
                        else
                        {
                            sb.Append(current);
                            reader.Read();
                            break;
                        }
                    }
                    else
                    {
                        sb.Append(current);
                    }
                }
                if (string.IsNullOrWhiteSpace((input = reader.ReadToEnd().Trim()))) return null;
                return sb.ToString();
            }
        }
    }
}