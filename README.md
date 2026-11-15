# PlayerManager (English)

An unofficial English translation of the **PlayerManager v5.1.0** BepInEx plugin for **Stick Fight: The Game** (plugin ID `stick.plugins.playermanager`). The original is a Chinese mod and isn't mine.

The source in `src/` was decompiled from the original DLL (kept in `original/`) and rebuilt. Changes from the original:

- **English text:** all 61 Chinese strings (buttons, lobby browser, config descriptions, log messages) translated to English.
- **Force kick:** the Kick button also sends a "client init refused" message, so players whose mods block kick messages (e.g. QOL-Mod) are still removed from the lobby.
- **AntiKick merged in:** with `BlockKicks` on (the default), forced removals are blocked too, not just kick messages. A "client init refused" message is ignored once you're in the game, and from anyone but the lobby owner; a host refusing you while you're joining still goes through, so a refused join fails normally. Blocked attempts are logged and shown in chat. Installing deletes the standalone `AntiKick.dll`.

It keeps the original plugin ID, so it replaces the Chinese DLL and keeps its config. Delete `stick.plugins.playermanager-v5.1.0.dll` from `BepInEx/plugins` when installing.

## Build

Requires the .NET SDK and a copy of the game with BepInEx 5 installed (none of the game files are included):

```sh
dotnet build -c Release -p:GameDir=/path/to/StickFightTheGame/
```

The built DLL is copied into that install's `BepInEx/plugins` folder.
