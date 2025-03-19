# RF.ScoreRanks
 A Rhythm Festival mod to display what score ranks you've achieved on each song.
 
  <a href="https://shorturl.at/9KExC"> <img src="Resources/InstallButton.png" alt="One-click Install using the Taiko Mod Manager" width="256"/> </a>
  
 Also requires the NijiiroScoring mod:
 
  <a href="https://shorturl.at/C1Ixs"> <img src="Resources/InstallButton.png" alt="One-click Install using the Taiko Mod Manager" width="256"/> </a>
  
The assets will need to be placed in an Assets directory (location defined in the config file), and layed out as such:
```
    Assets/
    ├── Big/
    │   ├── None.png
    │   ├── WhiteIki.png
    │   ├── BronzeIki.png
    │   ├── SilverIki.png
    │   ├── GoldMiyabi.png
    │   ├── PinkMiyabi.png
    │   ├── PurpleMiyabi.png
    │   └── Kiwami.py
    └── Small/
        ├── None.png
        ├── WhiteIki.png
        ├── BronzeIki.png
        ├── SilverIki.png
        ├── GoldMiyabi.png
        ├── PinkMiyabi.png
        ├── PurpleMiyabi.png
        └── Kiwami.py
```


# Requirements
 Visual Studio 2022 or newer\
 Taiko no Tatsujin: Rhythm Festival
 

# Build
 Install [BepInEx 6.0.0-pre.2](https://github.com/BepInEx/BepInEx/releases/tag/v6.0.0-pre.2) into your Rhythm Festival directory and launch the game.\
 This will generate all the dummy dlls in the interop folder that will be used as references.\
 Make sure you install the Unity.IL2CPP-win-x64 version.\
 Newer versions of BepInEx could have breaking API changes until the first stable v6 release, so those are not recommended at this time.
 
 Attempt to build the project, or copy the .csproj.user file from the Resources file to the same directory as the .csproj file.\
 Edit the .csproj.user file and place your Rhythm Festival file location in the "GameDir" variable.


# Links 
 [My Other Rhythm Festival Mods](https://docs.google.com/spreadsheets/d/1xY_WANKpkE-bKQwPG4UApcrJUG5trrNrbycJQSOia0c)\
 [My Taiko Discord Server](https://discord.gg/6Bjf2xP)
 
