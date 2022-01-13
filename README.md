# MusicBot
A Discord Music Bot

This Music Bot(MusicBear) is a discord bot coded in C# with Discord.Net.  
(Using library [BearMarkupLanguageLib](https://github.com/BearOffice/BearMarkupLanguageLib) to read configs.)  
This bot can send audio to different guilds at the same time.  
dotnet6.0 is needed.  

# Install
The folder where this app is located should include config.txt, libsodium.dll, opus.dll and can include a folder named playlist to store playlists.  
Before launching this app, ">token there<" should be replaced to bot's token in config.txt.  
  
A playlist should be saved in txt format, and the format of the contents should be like as follows:  
C:\Users\User\Music\example.mp3  
D:\example.flac  
...  

# Commands :
(the words in brackets are the abbreviation)  
  
<pre>
-help    show the help message  
-ping    check the bot's latency  
-userinfo    show the user's info  
-setgame [game]    set the bot's game  
-setstatus [status]    set the bot's status  
-playlist(pl)   show the available playlists  
-shutdown(exit/disconnect)    shut down the bot  

-join(j)    connect the bot to voice channel  
-play(p) [path/playlist]   play the provided song or playlist  
-playnext(pn) [path/playlist]   play the provided song or playlist next  
-movetonext(mn) [position]    move this song to the top of the queue  
-nowplaying(np)    show the current song  
-skip(s)    skip the current song  
-queue(q)    show the current queue  
-shuffle(sf)    shuffle songs in the queue  
-remove(rm) [position]    remove a song from the queue  
-removeall(rma)    remove all songs from the queue  
-stop    stop the current song and clear the queue  
-leave    disconnect the bot from voice channel  
<pre>
