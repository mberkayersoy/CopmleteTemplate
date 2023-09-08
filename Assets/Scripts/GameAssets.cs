using UnityEngine;

public class GameAssets
{
    private static GameAssets instance;
    
    public static GameAssets Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameAssets();
            }
            return instance;
        }
    }

    private GameAssets() {} // Gizli bir constructor, sadece bu sýnýf içinde bir örneði olmasýný saðlar.

    public GameObject GetAsset(string assetName)
    {
        return Resources.Load<GameObject>(assetName);
    }
}