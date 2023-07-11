2D Lighting

https://unity.com/how-to/2d-light-shadow-techniques-in-the-universal-render-pipeline

https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@14.0/manual/Lights-2D-intro.html

https://forum.unity.com/threads/script-for-generating-shadowcaster2ds-for-tilemaps.906767/

https://forum.unity.com/threads/script-for-generating-shadowcaster2ds-for-tilemaps.906767/page-2
1. choose your tilemap from the hierarchy.
2. if you already have "ShadowCaster2D" here, remove it. They will be created automatically later.
3. add Composite Collider2D, but keep the Tilemap Collider 2D.
4. a Rigidbody 2D is created, set Body Type to "Static".
5. check "Used by Composite" in Tilemap Collider 2D.
6. create a new C# script, paste the first code from post 1. Save it. Confirm the query.
7. in the Unity top bar now you will find a "Tools" tab, select "Generates ShadowCasters 2D" from there.

