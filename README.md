# League Summoner Name Checker

> C#/.NET library and console utility for checking the availability of summoner names for League of Legends

## Features
- Check the availability of multiple names in bulk on any game server
- If a name is unavailable, see when the account was last played on and when the name will become available
- Display the results in a table
- Export results to CSV

![screenshot](screenshot.png)

## Usage
1. Download the latest `league-summoner-name-checker-x.x.x.zip` (where x.x.x is the version number) on the [Releases](https://github.com/GeorgeGee/league-summoner-name-checker/releases) page and extract the contents into a folder
2. Create a text file `names.txt` inside the same folder, containing summoner names to check availability for (line separated)
3. Open PowerShell or Command Prompt and change directory to the folder which contains the downloaded files
4. Run the following command:  
`.\SummonerNameCheckerConsole.exe --apikey "API_KEY" --input "names.txt" --server "euw1"`
> Replace `API_KEY` with your Riot Games API key. Get one for free at https://developer.riotgames.com

## Arguments
Name|Type|Description
-|-|-
`-a` or `--apikey`|String|**Required.** Riot Games API key. Get one for free at https://developer.riotgames.com 
`-i` or `--input`|String|**Required.** Input `.txt` file path
`-o` or `--output`|String|*Optional.* Output `.csv` file path
`-s` or `--server`|String|*Optional.* League of Legends game server code (see below). Default is `euw1` (EU West)

Server|Code
-|-
Europe West|`euw1`
Europe Nordic & East|`eun1`
North America|`na1`
Latin America North|`la1`
Latin America South|`la2`
Brazil|`br1`
Japan|`jp1`
Russia|`ru`
Turkey|`tr1`
Oceania|`oc1`
Republic of Korea|`kr`