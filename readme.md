[![.NET](https://github.com/badHax/TradingViewScanner/actions/workflows/dotnet.yml/badge.svg)](https://github.com/badHax/TradingViewScanner/actions/workflows/dotnet.yml)

### Overview
this is a clug of a project that I made to scan for crypto coins on tradingview since because the actual page does not work. Also added desktop and push alerts. It uses the TradingView API to get the stock data and then filters it based on the criteria you set.

### Run
1. Clone the repo
2. Add setting values to appsettings.json or user secrets in the API project
3. Run `docker compose -f docker-compose.yml up --build` in the root directory
