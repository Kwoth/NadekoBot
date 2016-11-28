using System.IO;

namespace NadekoBot.Classes.JSONModels {
    public class LocalizedStrings {
        public string[] Insults { get; set; } = {
            " If I wanted to kill myself, I would climb up your ego and jump down to your IQ level.",
            " Your momma’s so fat her patronus is a cake.",
            " I can explain it to you, but I can’t understand it for you.",
            " Stop singing! You sound like an old granny being eaten by a shark!",
            " You are about as useful as a knitted condom.",
            " Did you know they used to call trampolines ‘jumpolines’ until your mom jumped on one?",
            " I can't tell if you're on too many drugs or not enough.",
            " winkle twinkle little star, I want to hit you with my car, Throw you off a cliff so high, I hope you break your neck and die.",
            " I failed a spelling test because they asked me how to spell 'bitch' and I wrote down your name.",
            " I thought of you today. It reminded me to take out the garbage.",
            " You're not my cup of tea, mainly because I don't like huge pieces of shit in my tea.",
            " It's called FUCK OFF and it's located over there.",
            " Your mum and dad hated you so much, your bath toys were an iron and a toaster.",
            " I'm not saying I hate you, but if you ever got hit by a bus, I'd probably be the one driving it.",
            " I'm a pacifist alright - I'm about to pass a fist right across your face.",
            " I'm not saying I hate you, but if you were on fire, I'd sit down and pull out the marshmallows.",
            " Nothing happens after you die? False. Some of us will be throwing a party.",
            " I don't hate you, but I wish your dad used a condom.",
            " Interrupt my sleep and I'll interrupt your breathing.",
            " You need to go brush your teeth cause all you seem to do is talk shit!",
            " Shut up, you failed result of an abortion.",
            " Someone up there must have a sense of humor because you're a joke.",
            " I'm the type of person to laugh at mistakes so sorry if I laugh at your face.",
            " You're so pathetic, your imaginary friend hates you.",
            " At least my birth certificate isn't an apology letter from the Trojan Co.",
            " If you don't like the way I drive, stay off the sidewalk."
        };

        public string[] Praises { get; set; } = {
            " You are cool.",
            " You are nice!",
            " You did a good job.",
            " You did something nice.",
            " is awesome!",
            " Wow."
        };

        public static string[] GetAvailableLocales() {
            Directory.CreateDirectory("data/locales");
            return Directory.GetFiles("data/locales");
        }

        //public static void HandleLocalization() {
        //    var locales = LocalizedStrings.GetAvailableLocales();


        //    Console.WriteLine("Pick a language:\n" +
        //                      "1. English");
        //    for (var i = 0; i < locales.Length; i++) {
        //        Console.WriteLine((i + 2) + ". " + Path.GetFileNameWithoutExtension(locales[i]));
        //    }
        //    File.WriteAllText("data/locales/english.json", JsonConvert.SerializeObject(new LocalizedStrings(), Formatting.Indented));
        //    try {
        //        Console.WriteLine($"Type in a number from {1} to {locales.Length + 1}\n");
        //        var input = Console.ReadLine();
        //        if (input != "1")
        //            Locale = LocalizedStrings.LoadLocale(locales[int.Parse(input) - 2]);
        //    } catch (Exception ex) {
        //        Console.ForegroundColor = ConsoleColor.Red;
        //        Console.WriteLine(ex);
        //        Console.ReadKey();
        //        return;
        //    }
        //}

        public static LocalizedStrings LoadLocale(string localeFile) =>
            Newtonsoft.Json.JsonConvert.DeserializeObject<LocalizedStrings>(File.ReadAllText(localeFile));
    }
}
