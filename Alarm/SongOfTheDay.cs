namespace WingTechBot.Alarm;

internal static class SongOfTheDay
{
    private static readonly string[] _sotdOptions = new[]
    {
            "Song of the morning, Darwin Derby by Vulfpeck: https://www.youtube.com/watch?v=WrdsotPDrRg",
            "Song of the morning, Love Today by MIKA: https://www.youtube.com/watch?v=AWiccrTB4LM",
            "Song of the morning, Virtual Insanity by Jamiroquai (my favorite!): https://www.youtube.com/watch?v=4JkIs37a2JE",
            "Song of the morning, Baba Yetu by Christopher Tin: https://www.youtube.com/watch?v=IJiHDmyhE1A",
            "Song of the morning, Sogno di Volare by Christopher Tin: https://www.youtube.com/watch?v=WQYN2P3E06s&vl=en",
            "Song of the morning, Waiting For My Real Life To Begin by Colin Hay: https://www.youtube.com/watch?v=wQBHPn9sDfY",
            "Song of the morning, Overkill by Men At Work: https://www.youtube.com/watch?v=RY7S6EgSlCI",
            "Song of the morning, Miracle by Caravan Palace: https://www.youtube.com/watch?v=XRP9k9nlAfE (or ||https://www.youtube.com/watch?v=QdabIfmcqSQ|| if you're feeling risqué (NSFW))",
            "Song of the morning, See the Day by The Altogether: https://www.youtube.com/watch?v=LIkS2lbbc7I",
            "Song of the morning, I Won't Let You Down by OK GO: https://www.youtube.com/watch?v=u1ZB_rGFyeU",
            "Song of the morning, Mr. Brightside by The Killers: https://www.youtube.com/watch?v=gGdGFtwCNBE",
            "Song of the morning, El Dorado by Stellar: https://www.youtube.com/watch?v=1KdQvhlINIk",
            "Song of the morning, Heat Waves by Glass Animals: https://www.youtube.com/watch?v=mRD0-GxqHVo",
            "Song of the morning, Another Day Of Sun from La La Land (you really should watch this movie): https://www.youtube.com/watch?v=xVVqlm8Fq3Y",
            "Song of the morning, Wanna Be Her by june: https://www.youtube.com/watch?v=PlzegsqA-6c",
            "Song of the morning, Space Oddity by David Bowie: https://www.youtube.com/watch?v=iYYRH4apXDo",
            "Song of the morning, Seaside Rendezvous by Queen: https://www.youtube.com/watch?v=36nqGs_Dvws",
            "Song of the morning, Seven Seas of Rhye by Queen: https://www.youtube.com/watch?v=FxIo57WURRE",
            "Song of the morning, Don't Stop Believin' by Journey: https://www.youtube.com/watch?v=1k8craCGpgs",
            "Song of the morning, Tiny Dancer by Elton John: https://www.youtube.com/watch?v=yYcyacLRPNs",
            "Song of the morning, Video Killed The Radio Star by The Buggles: https://www.youtube.com/watch?v=W8r-tXRLazs",
            "Song of the morning, Burning Heart by Survivor: https://www.youtube.com/watch?v=Kc71KZG87X4",
            "Song of the morning, Uptown Girl by Billy Joel: https://www.youtube.com/watch?v=hCuMWrfXG4E",
            "Song of the morning, Vienna by Billy Joel: https://www.youtube.com/watch?v=wccRif2DaGs",
            "Song of the morning, Sleeping With the Television On by Billy Joel: https://www.youtube.com/watch?v=G7lpvVf2rCY",
            "Song of the morning, 9 To 5 by Dolly Parton: https://www.youtube.com/watch?v=UbxUSsFXYo4",
            "Song of the morning, The Rubberband Man by The Spinners: https://www.youtube.com/watch?v=KSMVflSBKx8",
            "Song of the morning, Highway to Hell by AC/DC: https://www.youtube.com/watch?v=l482T0yNkeo",
            "Song of the morning, Back In Black by AC/DC: https://www.youtube.com/watch?v=pAgnJDJN4VA",
            "Song of the morning, Come On Eileen by Dexys Midnight Runners: https://www.youtube.com/watch?v=GbpnAGajyMc",
            "Song of the morning, Peace of Mind by Boston: https://www.youtube.com/watch?v=edwk-8KJ1Js",
            "Song of the morning, Fortunate Son by Creedence Clearwater Revival: https://www.youtube.com/watch?v=ec0XKhAHR5I",
            "Song of the morning, Take Me Home, Country Roads by John Denver: https://www.youtube.com/watch?v=1vrEljMfXYo",
            "Song of the morning, Break My Stride by Matthew Wilder: https://www.youtube.com/watch?v=B4c_SkROzzo",
            "Song of the morning, I've Just Seen A Face by The Beatles: https://www.youtube.com/watch?v=m8LbJfC0SYM",
            "Song of the morning, Back In The U.S.S.R by The Beatles: https://www.youtube.com/watch?v=dc2tJqnkLLA",
            "Song of the morning, Paperback Writer by The Beatles: https://www.youtube.com/watch?v=yYvkICbTZIQ",
            "Song of the morning, Footloose by Kenny Loggins: https://www.youtube.com/watch?v=ltrMfT4Qz5Y",
            "Song of the morning, Superstition by Stevie Wonder: https://www.youtube.com/watch?v=ftdZ363R9kQ",
            "Song of the morning, Cats in the Cradle by Harry Chapin: https://www.youtube.com/watch?v=494Wr7On6bA",
            "Song of the morning, Carry On Wayward Son by Kansas: https://www.youtube.com/watch?v=2X_2IdybTV0",
            "Song of the morning, Rich Girl by Hall and Oates: https://www.youtube.com/watch?v=oIAkRVBS-0U",
            "Song of the morning, YMCA by Village People: https://www.youtube.com/watch?v=CS9OO0S5w2k",
            "Song of the morning, Macho Man by Village People: https://www.youtube.com/watch?v=_bLRaGo-Qwc",
            "Song of the morning, Evil Ways by Santana: https://www.youtube.com/watch?v=_tKIPuLfeKg",
    };

    public static string GetSong() => _sotdOptions[Program.Random.Next(Length)];

    public static string GetSong(int x) => _sotdOptions[x];

    public static int Length => _sotdOptions.Length;
}
