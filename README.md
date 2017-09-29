# Localiza
Localiza is a Unity plugin to localize your game/app.

![Screenshot](https://i.imgur.com/lE3Um4c.png)

## Features 
- **Grouping**: Use Group to manage your localization items, and it does not affect how you access items .
- **Localize (Almost) everything**: Anything that inherits UnityEngine.Object can be localized, just modify the config file.
![Imgur](https://i.imgur.com/KKnoWXU.png)
- **Referenece Mode**: Easiliy check your base localization when translating!
![Imgur](https://i.imgur.com/IeDbZNa.png)
- **Recycle Bin**: Get back the items you accidentally deleted!

## Built-In Runtime Components
- **LocalizationCache** as a access interface to localization file.
- **UITextSync** to auto change UI.Text component's text according activated localization.

## How To Install
- Download the repo and extract the "LocalizaV2" Folder in Assets to wherever in your Unity project Assets folder

## How To Use
![Imgur](https://i.imgur.com/E0FRy5A.png)

## Interface Tips
![Imgur](https://i.imgur.com/R6f2EIx.png)

## Some Description
- The Localiza window reads what you want to localizae from config file and get the types via reflection.
- You have to provide Drawer (inherits **ItemDrawerBase**) for any additional types (Assets) you want to localize and add it to config.
- The config file is located at LocalizaV2/Editor.
- By default, Localiza provide Drawer for String, AudioClip, Sprite Drawers.

