# Crossbell Translation Tool
A tool to assist with translating the PSP game Ao no Kiseki.

# Thank You!
This tool can only exist due to https://github.com/Ouroboros/EDDecompiler/

## What you can extract/translate with this tool
+ Items
+ Equipment
+ Fish
+ Food
+ Location Names
+ Quest text in the Detective's Notebook
+ Magic
+ Crafts
+ Books / Newspapers
+ NPC Names
+ Character Names
+ Scenario Text / Dialog
+ Monster Text - Names / Descriptions / Ability Names

## What you cannot extract/translate with this tool
+ EBOOT.BIN Text - Mostly the game interface
+ Text in images

## How to Extract Text 
First, download the translation tool at the link below and extract the zip somewhere. I recommend its own directory.  
Next, open a command prompt and go to directory where you extracted the files. Run the command:
`OmegaX.AoNoKisekiTranslation.exe extract ao [path to the Ao no Kiseki iso] [output directory path]`

This is place a bunch of files in the output directory path.
The json files in the 'scena' directory will be mostly dialog text while the json files in the 'text' will be everything else.  
The 'stringtable.json' file in the root output directory will contain mostly NPC names.

## Translating Text
In each json file created, there are pairs of lines, Text and Translation. Don't touch the value of the Text line.  Just enter your translation as the value of the Translation line. If you don't have a translation, just leave the Translation value as "". This will tell the program not to change that line of text.  
You will notice in the scenario files that many lines where the Text value looks to be a filename or already English text. DO NOT CHANGE THESE LINES. You will break the game.

## Encoded Strings in Scenario Text
Many lines of text in the scenario files have non-text data in them. Here is an example:  
`"#30W#40A七耀暦１２０４年──某月\u0006\u0002"`

Be sure to keep those in you translations. The # codes at the beginning of the text looks to do things like show a character portrait or play a sound file. The \u codes do other things. \u0001 is a line break and \u0002 tells the game to wait for you to press a button before closing the dialog window.

## How to Inject Text
First of all, injecting text will create a new ISO file, so you original one will not be changed.  
Open a command prompt and go to directory where you extracted the files. Run the command:
`OmegaX.AoNoKisekiTranslation.exe build ao [path to the Ao no Kiseki ISO] [path to the new ISO] [output directory path used the extract command]`

Again, don't forget "" around the paths.
 
## EBOOT.BIN
You can use this tool to inject an decrypted EBOOT.BIN into the translated ISO. When building a translated ISO, if the tool sees an EBOOT.BIN file in the root translation directory, it will automatically use it in the build.
 
### EBOOT.BIN 'Noel' Patch
Ao no Kiseki has a hack that changes the name of Noel from "ノエル曹長 (Sgt. Major Noel)" to "ノエル (Noel)". This takes place duing the ending scenes of the prologue. Because the name change takes place in code, normal translation does not effect this. If the build is given a decrypted EBOOT.BIN, it will patch the file, translated Noel's name. If affected her name & dialog and the description text for saves.
 
### How to Get an Decrypted EBOOT.BIN With PPSSPP
+ Open PPSSPP
+ Open the Settings Menu
+ Open the Tools Menu
+ Select Developer Tools
+ Enable 'Dump decrypted EBOOT.BIN on game boot
+ Run the game.
+ The EBOOT.BIN file will be found at [PPSSPP directory]\memstick\PSP\SYSTEM\DUMP\NPJH50473.BIN
+ Copy the file to the base of the translation directory and rename it EBOOT.BIN 
 
## Download Links
+ Crossbell Translation Tool v1.2.0: https://github.com/FrantzX/CrossbellTranslationTool/releases/download/v1.2.0/CrossbellTranslationTool.zip
+ Partially Translated Output Files v1.1.0: https://github.com/FrantzX/CrossbellTranslationTool/releases/download/v1.1.0/Ao.no.Kiseki.Translation.zip

## Already Translated in the Partially Translated Output Files
+ The ENTIRE prologue!
+ Items
+ Equipment
+ Fish
+ Food
+ Location Names
+ Magic
+ Crafts
+ SOME NPC Names
+ Character Names
+ Treasure Chests
+ Orbment Recharging Stations
+ Random Item Drops in the Field
