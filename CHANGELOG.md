## 2.0.1 (12/06/2022)
* Fixed the help screen not displaying when invalid arguments are provided.

## 2.0.0 (12/06/2022)
* Updated to use latest Riot Games API endpoints.

## 1.2.2 (19/07/2020)
* Fixed null reference exceptions caused by failed API requests for the summoner or their match history where the response code was not 200 OK but an exception was not thrown.

## 1.2.1 (03/09/2019)
- Added a new Summoner name availability status for an account that exists but has played no matches, i.e. unknown - the name might be available.
- Added support for sorting the results in the table and CSV.
- Added MIT license.
- Fixed the console utility stating that no summoners were retrieved if no output file path was specified.
- Fixed the results table only being printed if an output file path was specified.

## 1.2.0 (07/05/2019)
- Fixed an issue where checking the availability of a large quantity of names would fail once a rate limit was hit.

## 1.1.0 (02/02/2019)
- Added support for exporting results to CSV file.
- Added Server/ServerCode conversion extension methods and ApiHelper overload for string serverCode parameter.
- Added documentation for Riot Games article on name expiry calculation.
- Removed command line argument for displaying results in a table (instead it is now the default behaviour).
- Retargeted .NET Framework 4.7.2 and updated NuGet packages.

## 1.0.0 (28/01/2019)
- Initial release.
