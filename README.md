Azure for Unity3D Example
=================================

This is a fork of a fork of [bitrave's Unity3D Azure plugin](https://github.com/bitrave/azure-mobile-services-for-unity3d) with the following goals:

1. Use the free and open source version of [Json.NET](http://www.newtonsoft.com/json) (not the paid Unity asset store version)
2. Have a proper gitignore for a Unity project so there aren't broken script references
3. Provide a well laid out example with the new UI that will work on mobile
4. Be updated to Unity 5.x
5. Remove implicit vars
6. Support as many platforms as possible

## This has been tested and built for
- Windows 10
- Android 5.1
- OSX 10.10

## Tutorial
See [tutorial.md](tutorial.md) for a step by step walkthrough

## Limitations
- Delete callback hasn't been implemented [issue](https://github.com/bitrave/azure-mobile-services-for-unity3d/issues/31)
- You can't update unity objects & UI from within the callback, so flags are set and checked in Update() This is inneficient but works. If you know a better way please [let me know!](https://github.com/Frozenfire92/azure-mobile-services-for-unity3d/issues/new)
- I didn't carry over the facebook example from the original repo. They are just using an access token and don't demo the login process. Facebook also provides a [Unity SDK](https://developers.facebook.com/docs/unity). If you are interested check out [this tutorial](http://www.deadlyfingers.net/azure/unity3d-game-dev-with-azure-mobile-services-using-bitrave-plugin/)

## Taking it further
Here are some ideas to get you going
- Cache the results to save on API calls
- Create something more complex than a leaderboard
- Create a login/profile system

## Thanks
- Bitrave
- Vaughan Knight
- Deadlyfingers
  - [Leaderboard tutorial](http://www.deadlyfingers.net/azure/unity3d-leaderboard-demo-using-bitrave-azure-plugin/) - [Facebook & todo tutorial](http://www.deadlyfingers.net/azure/unity3d-game-dev-with-azure-mobile-services-using-bitrave-plugin/)
