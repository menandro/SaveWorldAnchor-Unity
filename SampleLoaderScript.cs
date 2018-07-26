using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class SampleLoaderScript : MonoBehaviour {
	void Start(){
		string filename = "sample.dat";
		string anchorName = "sample";
		SaveWorldAnchorGlobal worldAnchorSaver = new SaveWorldAnchorGlobal();
        worldAnchorSaver.SetWorldAnchor(filename, anchorName, this.gameObject); //Non blocking
	}
}
