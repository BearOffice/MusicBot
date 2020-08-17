# MusicBot
A Discord Music Bot

This MusicBot(MusicBear) is a discord bot coded in C# with [Discord.Net](https://github.com/discord-net/Discord.Net)  
This Bot can send audio to different guilds at the same time.  

# Install
The folder where this app is located must include "AppConfig.xml", "Help.txt", and can include a "Playlist" folder to store playlists.   
Before you launch this app, you should replace "token there" to your bot's token in "AppConfig.xml".  

A playlist must be saved in txt format, and the format of the contents should be like as follows:  
C:\Users\User\Music\example.mp3  
D:\example.flac  
...  

# MusicBot
Discord Music Bot

このMusicBot（MusicBear）は，[Discord.Net](https://github.com/discord-net/Discord.Net)を使用し，C＃でコーディングされたものです．  
このBotでは複数のギルドに音声を送信できます．  

# インストール
このアプリが置かれているフォルダーには，「AppConfig.xml」と「Help.txt」が含まれていることが必要です．また，プレイリストを保存するための「Playlist」フォルダーを含めることができます．  
このアプリを起動する前に、「AppConfig.xml」の中の「token there」をあなたのbotのトークンに置き換える必要があります．  

プレイリストではtxt形式で保存する必要があり，その中のフォーマットでは次のように書く必要があります：  
C:\Users\User\Music\example.mp3  
D:\example.flac  

# Commands :
(the words in brackets are the abbreviation)  
  
-help    show the help message  
-ping    check the bot's latency  
-userinfo    show the user's info  
-setgame [game]    set the bot's game  
-setstatus [status]    set the bot's status  
-showplaylists(spl)    shows the available playlists  
-updateplaylists(upl)    update the playlists  
-shutdown(exit/disconnect)    shut the bot down  

-join(j)    connect the bot to voice channel  
-play(p) [path]    play the provided song  
-playnext(pn) [path]    play the provided song next  
-playlist(pl)    play the provided playlist's song  
-movetonext(mvn) [position]    move this song to the top of the queue  
-nowplaying(np)    show the currently playing song  
-skip(s)    skip the current song  
-queue(q)    show the current queue  
-shuffle(sf)    shuffle songs that have added  
-remove(rm) [position]    remove a song from the queue  
-removeall(rma)    remove all songs from the queue  
-stop(sp)    stop the current song and clear the queue  
-leave(l)    disconnect the bot from voice channel  
