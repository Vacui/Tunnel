using Level;
using UnityEngine;

public class Testing : MonoBehaviour
{
    private int currentIndex = -1;
    private int CurrentIndex
    {
        get { return currentIndex; }
        set
        {
            if (useAutoLevelGeneration)
            {
                currentIndex = Mathf.Clamp(value, 0, 30);
                GenerateLevel(currentIndex);
            } else
            {
                currentIndex = Mathf.Clamp(value, 0, seeds.Length - 1);
                LevelManager.main.LoadLevel(seeds[currentIndex]);
            }
        }
    }
    private bool useAutoLevelGeneration;

    private string[] seeds = new string[]{
        "1/1/1",
        "3/1/1-5-3",
        "3/2/1-5-3-0-0-0",
        "3/3/5-5-3-4-7-7-1-5-4",
        "4/4/3-7-7-7--6-7-2--6--4-1-5-5-4",
        "5/5/6-7-7-7-7-6--0--4-6--0--4-6-1--0-4-5-5-5-5-4",
        "5/5/1-7-7-7-7-6-0-0-0-4-6-0-0-0-4-6-0-0-0-4-5-5-5-5-4",
        "14/11/1-5-5-2-5-5-2-7-7-7-7-7-7-7-4-0-0-6-0-0-6-0-0-0-0-4-0-4-4-0-0-6-0-0-2-5-5-5-5-4-0-4-4-0-0-6-0-0-6-0-0-4-7-7-2-4-4-7-7-2-0-0-5-5-5-5-6-0-4-0-4-0-0-5-5-5-5-2-5-6-6-0-4-0-4-0-0-0-0-0-0-6-0-6-6-0-4-0-4-0-0-0-6-7-7-2-7-7-6-0-4-0-4-0-0-6-7-7-2-7-7-7-2-5-4-0-4-0-0-6-0-0-6-0-0-0-0-0-0-0-4-7-7-7-0-0-5-5-5-5-3-0-0-0",
        //"3/3/1-4-3-3-4-3-4-5-2",
        //"3/3/1-5-4-4-6-4-5-6-2",
        //"4/3/1-8-5-8-5-7-3-3-0-4-7-2",
        //"4/6/1-4-8-2-3-0-3-3-3-3-3-3-6-7-3-3-5-4-0-3-6-4-4-7",
        //"14/11/--5-4-4-4-2-4-4-0-4-4-4-8---3----3---3----3-5-4-9-4-0-4-9-4-4-9-4-0-4-7-3--3--3--3---3-----6-4-0--3--3---0-4-4-4-8--5-4-4-9-4-0-4-4-4-4-8--3-5-9-8--3--5-4-4-4-0-3--3-3-3-6-4-0--3----3-3--3-3-0-4-4-4-4-9-4-0-4-7-3--3-3------3--3---3--3-1-4-4-4-4-4-0-4-7---0-4-7",
        //"14/11/5-4-4-4-4-0-4-8----5-8-0-3-5-8---3--3----3-6-2-3-3-6-4-4-7-5-9-4-4-4-0-4-8-3-0-4-4-4-4-9-7-5-4-4-0-4-7-3-3-5-4-4-8-3--3---3---3-3-3---3-6-4-0-4-8-3---3-3-0-4-4-1-4-4-4-8-6-9-4-8-3-3-3---3--0-4-9-4-7--3-3-3-3---3--3--3----3-3-6-7---3--3--0-4-8--3-6-4-4-4-4-0-4-7----6-4-0",
        //"14/11/5-4-8-0-4-4-4-4-8---0-4-8-3--3-3-----6-4-0-3--3-3--6-9-4-4-4-0-4-4-4-9-4-7-0---6-4-4-8----5-7---3-0-4-4-4-4-9-4-0-4-7-5-4-8-3-3-5-4-8--3-----3--3-3-3-3--3--0-4-4-4-4-0-4-7-3-3-0--3--3--0-4-4-4-4-8-3-3-3--3--3--3--5-4-8-3-2-6-9-4-7--3--6-4-7--3-3---6-4-4-4-0-4-4-4-4-4-7-1",
        //"14/11/--------5-2-4-4-4-8-5-4-4-0-4-4-4-4-9-9-4-0-4-7-3---3-----3-6-8-3---3-5-4-9-4-0-4-8-3--3-6-4-8-3-3--3--3--3-6-8-0-4-8-3-3-1-4-9-4-9-8-3--3-3--3-3-6-4-4-9-8-3-3-6-4-9-9-4-0-3--5-4-7-3-3-0-4-4-9-7---3-5-9-4-4-0-3-3-0-4-9-4-4-8-3-3-3----3-3-3--3---3-3-0-6-4-0-4-7-6-7--6-4-4-0-7"
    };

    [System.Flags]
    public enum TestEnum
    {
        None = 0,
        Enum1 = 1,
        Enum2 = 2,
        Enum3 = 4
    }

    [EnumFlag(EnumStyle.Button)] public TestEnum var0;

    [SerializeField, EnableIf(nameof(var0), TestEnum.None, TestMethod = ComparisionTestMethod.Equal)] private bool varN;
    [SerializeField, EnableIf(nameof(var0), TestEnum.Enum1 | TestEnum.Enum2, TestMethod = ComparisionTestMethod.Mask)] private bool var1;
    [SerializeField, EnableIf(nameof(var0), TestEnum.Enum2, TestMethod = ComparisionTestMethod.Mask)] private bool var2;
    [SerializeField, EnableIf(nameof(var0), TestEnum.Enum3, TestMethod = ComparisionTestMethod.Equal)] private bool var3;

    private void Start()
    {
        //levelManager.LoadLevel(new LevelManager.Seed("3/3/1-4-3-3-6-6-4-4-2"));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            CurrentIndex++;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            CurrentIndex--;
        if (Input.GetKeyDown(KeyCode.R))
            CurrentIndex = currentIndex;
    }

    private void GenerateLevel(int size)
    {
        int height = Mathf.Clamp(size, 0, 20);
        int width = Mathf.Clamp(size, 0, 30);
        Debug.Log($"{width}x{height}");
        string seed = $"{width}/{height}/1-";
        for (int i = 1; i < width * height; i++)
            seed += "2-";

        LevelManager.main.LoadLevel(seed.Trim('-'));
    }
}