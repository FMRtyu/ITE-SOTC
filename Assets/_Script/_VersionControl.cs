using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class _VersionControl : MonoBehaviourSingletonPersistent<_VersionControl> {

	Text versionText;

	// Use this for initialization
	void Start () {
		versionText = GetComponentInChildren<Text>();
		versionText.text = Application.version;
	}
	
	// Update is called once per frame
	//void Update () {
		
	//}
}
