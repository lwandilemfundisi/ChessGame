# ChessGame
Chess game built using DDD and Event Architecture

Hi guys

I appreciate the support, I see there has been quite a number of clones. This application is still in progress, day by day I am building it up. So the intension here is to build an
online chess game where 2 players can play online a chess a game ;-). Its a very interesting one so you want to be around for this, thanks!!!!!!

Regards
Lwandile

## Chess web client

The Next.js client lives in `chess-web`. It provides a clickable board, promotion selection, game creation/loading/reset, shareable game links, and a session move list. It sends all game operations to the Chess API through a same-origin Next.js route proxy.

### Run locally

1. Start SQL Server and run `Chess.Api` (`dotnet run --project Chess.Api`). The API applies its EF migrations at startup and listens on `https://localhost:5001` by default.
2. Trust the local ASP.NET HTTPS development certificate with `dotnet dev-certs https --trust`.
3. In `chess-web`, copy `.env.example` to `.env.local` and set `CHESS_API_BASE_URL` if the API uses a different address.
4. Use Node.js 20.9 or newer, run `npm install`, then `npm run dev` in `chess-web` and open `http://localhost:3000`.

The API must be running for game actions. The Next.js scripts use Node's system certificate store so the local proxy can follow the API's HTTPS redirect after the ASP.NET development certificate is trusted. Share the game link or ID with another person to play the same turn-based board; open boards sync through API polling about every 2.5 seconds. The client does not add accounts or matchmaking.
