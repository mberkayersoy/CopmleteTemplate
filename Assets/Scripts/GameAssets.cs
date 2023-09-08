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

    private GameAssets() {} // Gizli bir constructor, sadece bu s�n�f i�inde bir �rne�i olmas�n� sa�lar.

    public GameObject GetAsset(string assetName)
    {
        return Resources.Load<GameObject>(assetName);
    }
}