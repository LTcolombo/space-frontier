# space-frontier

# Idea
The game is meant to be a mix of classic RPG games and a coty builder mechanics.
The RPG part is built around players avatar which is an NFT, minted upon initial load of the game.
The players role is to manage a small settlement and optimise its energy supply, so that it can thrive and eventually allow to yield some rewards in a form of FT.
The avatar is able to communicate with NPC's using a comprehensive system of non linear dialogs and quests, allowing to reach objectives, build up players character (improving the 'karma' attribute of the NFT), as well as earn and spend, when needed, the fungible token.

# Tech
## Client
Client is built using Unity as a webGL build.
- this allows hassle-free connection to most wallets
- since the build size is relatively small (just over 15mb) it allows to start playing within seconds after clicking the link (no istalls etc).

## Backend
Typescript NodeJS. 
- wraps blockchain API.
- manages custodial wallets. In order to reduce friction the in-game FT is stored ona. custodial wallet (which the player can request to withdraw from (or deposit into))
- generates avatars
- manages mutable attributes of avatars
- controls any asset generation mechanics. (e.g. game quest which might produce some value are entirely controlled by backend)

## Blockchain
Currenlty deployed to near blockchain. Has 2 contracts:
- A pretty much standard FT token used in the game.
- A mutable avatar NFT contract. Used to modify the avatar's karma based on players decisions. Would also hold things like XP etc, making the players effort more meaningful (in making his avatar objectively more efficient in the game)
