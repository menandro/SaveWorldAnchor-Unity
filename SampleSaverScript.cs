using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class SampleSaverScript: MonoBehaviour {
    void Start () {
		string filename = "sample.dat";
		string anchorName = "sample";
        SaveWorldAnchorGlobal worldAnchorSaver = new SaveWorldAnchorGlobal();
		worldAnchorSaver.Save(filename, anchorName, this.GameObject);
    }
}
