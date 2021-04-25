using UnityEngine;

namespace Level
{
    [DisallowMultipleComponent]
    public class LevelGenerator : MonoBehaviour
    {
        public static LevelGenerator main;

        private void Awake()
        {
            if (main == null) main = this;
            else Destroy(this);
        }

        public void GenerateLevel(int width, int height)
        {
            Debug.Log($"Generating level {width}x{height}");
            string newLevelSeed = $"{width}/{height}/1";
            for (int i = 1; i < (width * height)-1; i++)
                newLevelSeed += "-2";
            newLevelSeed += "-0";

            //GridXY<TileType> newLevel = new GridXY<TileType>();
            //newLevel.CreateGridXY(width, height, LevelManager.CELLSIZE, new Vector2(width / 2.0f - 0.5f, height / 2.0f - 0.5f) * new Vector2(-1, 1) * LevelManager.CELLSIZE);
            //newLevel.SetAllTiles(TileType.NULL);

            //int startX = Random.Range(0, width - 1);
            //int startY = Random.Range(0, height - 1);

            //newLevel.SetTile(startX, startY, TileType.Player);
            //newLevel.SetTile(MyUtils.RandomWithExceptions(0, width - 1, new List<int>(1) { startX }), MyUtils.RandomWithExceptions(0, height - 1, new List<int>(1) { startY }), TileType.Goal);

            //string newLevelSeed = newLevel.ToSeedString();

            Debug.Log($"Generated seed: {newLevelSeed}");

            LevelManager.main.LoadLevel(newLevelSeed);
        }
    }
}