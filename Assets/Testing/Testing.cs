﻿using UnityEngine;

public class Testing : MonoBehaviour {

    public MapManager mapGeneration;

    private string[] seeds = new string[]{
        "1/1/1",
        "4/3/1-8-5-8-5-7-3-3-0-4-7-2",
        "4/6/1-4-8-2-3-0-3-3-3-3-3-3-6-7-3-3-5-4-0-3-6-4-4-7",
        "14/11/--5-4-4-4-2-4-4-0-4-4-4-8---3----3---3----3-5-4-9-4-0-4-9-4-4-9-4-0-4-7-3--3--3--3---3-----6-4-0--3--3---0-4-4-4-8--5-4-4-9-4-0-4-4-4-4-8--3-5-9-8--3--5-4-4-4-0-3--3-3-3-6-4-0--3----3-3--3-3-0-4-4-4-4-9-4-0-4-7-3--3-3------3--3---3--3-1-4-4-4-4-4-0-4-7---0-4-7",
        "14/11/5-4-4-4-4-0-4-8----5-8-0-3-5-8---3--3----3-6-2-3-3-6-4-4-7-5-9-4-4-4-0-4-8-3-0-4-4-4-4-9-7-5-4-4-0-4-7-3-3-5-4-4-8-3--3---3---3-3-3---3-6-4-0-4-8-3---3-3-0-4-4-1-4-4-4-8-6-9-4-8-3-3-3---3--0-4-9-4-7--3-3-3-3---3--3--3----3-3-6-7---3--3--0-4-8--3-6-4-4-4-4-0-4-7----6-4-0",
        "14/11/5-4-8-0-4-4-4-4-8---0-4-8-3--3-3-----6-4-0-3--3-3--6-9-4-4-4-0-4-4-4-9-4-7-0---6-4-4-8----5-7---3-0-4-4-4-4-9-4-0-4-7-5-4-8-3-3-5-4-8--3-----3--3-3-3-3--3--0-4-4-4-4-0-4-7-3-3-0--3--3--0-4-4-4-4-8-3-3-3--3--3--3--5-4-8-3-2-6-9-4-7--3--6-4-7--3-3---6-4-4-4-0-4-4-4-4-4-7-1",
        "14/11/--------5-2-4-4-4-8-5-4-4-0-4-4-4-4-9-9-4-0-4-7-3---3-----3-6-8-3---3-5-4-9-4-0-4-8-3--3-6-4-8-3-3--3--3--3-6-8-0-4-8-3-3-1-4-9-4-9-8-3--3-3--3-3-6-4-4-9-8-3-3-6-4-9-9-4-0-3--5-4-7-3-3-0-4-4-9-7---3-5-9-4-4-0-3-3-0-4-9-4-4-8-3-3-3----3-3-3--3---3-3-0-6-4-0-4-7-6-7--6-4-4-0-7"
    };

    private void Awake() {
        mapGeneration.LoadMapAround(seeds[6], Vector3.zero);
    }

}