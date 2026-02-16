# All Vanity

A GTFO mod that unlocks all Vanity items locally.  

The names of items that you do not own are colored with a slight red tint. (only if [NoBoosters](https://thunderstore.io/c/gtfo/p/AuriRex/NoBoosters/) is *not* installed)  

### You still earn new cosmetics by playing vanilla with this mod installed like usual.

If you're running this mod with [SimpleProgression](https://thunderstore.io/c/gtfo/p/AuriRex/Simple_Progression/) installed, all the unlocking is handled by SimpleProgression instead, as that allows for finer control of what to unlock.  
Check the readme of that mod if you're a rundown developer.

<details>
<summary>Extra little config thingie;</summary>

(Only applies to if *not* running either NoBoosters or SimpleProgression):  
If for whatever reason you want to only unlock a subset of items you can create a file called `AllVanity_UnlockList.json` in your `BepInEx/config/` folder with the contents being a json list of all the `VanityItemTemplateDataBlock` persistent IDs that you want to unlock:
```json
[50,53,57,58,59,70,85,99,102,103,105,107,108,109,121,149]
```
</details>

### Incompatible with 'DeviousLick'!!
This plugin won't load if DeviousLick is installed to avoid crashes.
