________________________________________________________________________________
*Thanks to @Flatbread and Mirai for making this guide, plus Lucy for amending it for EOL*
________________________________________________________________________________

### Setting Up NadekoBot on Windows
#### Prerequisites 
- 1) [NET Framework][NET Framework] 4.5.2 (or 4.6)
- 2) [FFMPEG][FFMPEG] 
- 3) Google Account
- 4) Soundcloud Account (if you want soundcloud support)
- 5) [7zip][7zip] (or whatever you are using, WinRar)
- 6) [Notepad++][Notepad++]

####Guide 

- Create a folder, name it `NadekoBot`.
- Head to the [Nadeko Releases Page][Releases] and download `NadekoBot.0.99.10.zip`.
- Copy `NadekoBot.0.99.10.zip` to the `Nadeko` (folder we created before) and extract everything.
- You should now see all of the Nadeko files extracted, including `NadekoBot.exe`

####Creating DiscordBot application
- Go to [DiscordApp][DiscordApp].
- Log in with your Discord account.
- On the left side, press `New Application`.
- Fill out the `App Name` (your bot's name, in this case), put the image you want, and add an app description(optional).
- Create the application.
- Once the application is created, click on `Create a Bot User` and confirm it.
- Keep this window open for now.

####Setting up Credentials.json file
- In our `NadekoBot` folder you should have `.json` file named `credentials_example.json`. (Note: If you do not see a **.json** after `credentials_example.json `, do not add the `**.json**`. You most likely have `"Hide file extensions"` enabled.)
- Rename `credentials_example.json` to `credentials.json`.
- Open the file with your [Notepad++][Notepad++].
- In there you will see fields like `Token`, `ClientId`, `BotId`, `OwnerIDs`.
- In your [DiscordApp][DiscordApp], under `Bot User` part, you will see the `Token:click to reveal` part, click to reveal it.
- Copy your bot's token, and put it between `" "` in your `credentials.json` file.
- Copy `Client ID` and replace it with the example one in your `credentials.json` in `Client ID` **and** `BotID` field.
- Save your `credentials.json` but keep it open. We need to put your `User ID` and owner.

####Inviting your bot to your server 
- [Invite Guide][Invite Guide]
- Create a new server in Discord.
- Copy your `Client ID` from your [DiscordApp][DiscordApp].
- Replace `12345678` in this link `https://discordapp.com/oauth2/authorize?client_id=12345678&scope=bot&permissions=66186303` with your `Client ID`.
- Link should look like this: `https://discordapp.com/oauth2/authorize?client_id=**YOUR_CLENT_ID**&scope=bot&permissions=66186303`.
- Go to newly created link and pick the server we created, and click `Authorize`.
- Bot should be added to your server.

####Starting the bot
- Enter your `NadekoBot` folder.
- Run `NadekoBot.exe` (Note: There is `NadekoBot.exe` and `NadekoBot.exe.config`, don't run the second one)
- Your bot should now be online in the server we added him to.
- Note: Your bot will be offline in case you close `NadekoBot.exe`.

####Setting up OwnerIds
- In the server where your bot is, in a text channel, type `.uid`
- Your `User ID` should show, copy it.
- Close `NadekoBot.exe`
- Replace your `User ID` in the `credentials.json` between `[ ]` and save the changes.
- Run `NadekoBot.exe` again.
- Now you are the bot owner.
- You can add `User IDs` from the other users by separating IDs with a comma if you want to have more owners.

________________________________________________________________________________

#### Setting Up NadekoBot For Music
##### Prerequisites
- 1) [FFMPEG][FFMPEG] installed.
- 2) Setting up API keys.

- Follow these steps on how to setup Google API keys:
    - Go to [Google Console][Google Console] and log in.
    - Create a new project (name does not matter). Once the project is created, go into "Enable and manage APIs."
    - Under the "Other Popular APIs" section, enable `URL Shortener API` and `Custom Search Api`. Under the `YouTube APIs` section, enable `YouTube Data API`.
    - On the left tab, access `Credentials`. Click `Create Credentials` button. Click on `API Key`. A new window will appear with your `Google API key`. 
    - Copy the key.
    - Open up `credentials.json`. 
    - For `"GoogleAPIKey"`, fill in with the new key we copied.
- Follow these steps on how to setup Soundcloud API key:
    - Go to [Soundcloud][Soundcloud]. 
    - Enter a name for the app and create it. 
    - You will need to fill out an application form to request access to Soundcloud API.
    - All requests for an API key must go through the review process, where applications will be reviewed on a case by case basis, in line with Soundcloud API Terms of Use. If your application is successful, you will receive an API key. 
    - You will see a page with the title of your app, and a field labeled `Client ID`. Copy the ID. 
    - In `credentials.json`, fill in `"SoundcloudClientID"` with the copied ID.
- Restart your computer.

####Manual `ffmpeg` setup 
**Do this step in case you were not able to install `ffmpeg` with the installer.**

- Create a folder named `ffmpeg` in your main Windows directory. We will use **C:\ffmpeg** (for our guide)
- Download FFMPEG through the link https://ffmpeg.zeranoe.com/builds/ (download static build)
- Extract it using `7zip` and place the folder `ffmpeg-xxxxx-git-xxxxx-xxxx-static` inside **C:\ffmpeg**
- Before proceeding, check out this gif to set up `ffmpeg` PATH correctly http://i.imgur.com/aR5l1Hn.gif *(thanks to PooPeePants#7135)*
- Go to My Computer, right click and select Properties. On the left tab, select Advanced System Settings. Under the Advanced tab, select Environmental Variables near the bottom. One of the variables should be called "Path". Add a semi-colon (;) to the end followed by your FFMPEG's **bin** install location (**for example C:\ffmpeg\ffmpeg-xxxxx-git-xxxxx-xxxx-static\bin**). Save and close.
- Setup your API keys as explained above.
- Restart your computer.

[NET Framework]: https://www.microsoft.com/en-us/download/details.aspx?id=48130
[FFMPEG]: https://github.com/Soundofdarkness/FFMPEG-Inst/releases
[7zip]: http://www.7-zip.org/download.html
[Updater]: https://github.com/Kwoth/NadekoUpdater/releases
[Releases]: https://github.com/Kwoth/NadekoBot/releases
[DiscordApp]: https://discordapp.com/developers/applications/me
[Notepad++]: https://notepad-plus-plus.org/
[Invite Guide]: http://discord.kongslien.net/guide.html
[Google Console]: https://console.developers.google.com
[Soundcloud]: https://soundcloud.com/you/apps/new
