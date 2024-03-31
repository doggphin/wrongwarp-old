# Wrong Warp

My first attempt at making my dream game, made in C# and Unity for fun between Fall 2021 - Summer 2022.

This was my introduction to serious game development which involved learning how to maintain a large codebase and much exploration of different aspects of game design and coding practices.

Features include:

 - A server-authoritative multiplayer model with client prediction based on [Valve's Source Multiplayer Networking article](https://developer.valvesoftware.com/wiki/Source_Multiplayer_Networking) which encoded/decoded highly space-efficient byte streams and ran logic through operation codes
 - [Portals that transmit visuals, raycasts and audio](https://www.youtube.com/watch?v=0VFI9qGvLxg), including [recursive audial echoes](https://www.youtube.com/watch?v=k0CLrdIbJzo)
 - A comprehensive server-authoritative inventory system including items with their own inventories using Unity's ScriptableObjects
 - Completely modular inventory structures using Unity's ScriptableObjects system


As I gradually moved away from the core features Unity and Mirror provided (eg, NetworkIdentities and GameObjects) by creating my own versions of them, this project was my inspiration to make my own game engine.

## Instructions

Intended to be used with Unity 2021.3.0f1.

Controls:
 - On the main menu, select 'Host' to host and 'Join' to join a host
 - Move with WASD and Space
 - Interact with E
 - Open inventory with Tab
 - Use server emotes with H and B

## Visuals

### World state synchronization
![Idle](visuals/visual1.gif)

### Fully networked items within inventories within items within inventories
![2](visuals/visual2.png)

### Video 1: Portal echoes
[![Video 1](visuals/Screenshot_1.png)](https://www.youtube.com/watch?v=0VFI9qGvLxg)

### Video 2: Portal audio
[![Video 2](visuals/Screenshot_2.png)](https://www.youtube.com/watch?v=k0CLrdIbJzo)
