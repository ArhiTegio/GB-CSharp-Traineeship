﻿using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

class TinyRenderingTest {

	[Test]
	public void EditorSampleTestSimplePasses() {
	}

	[UnityTest]
	public IEnumerator EditorSampleTestWithEnumeratorPasses() {
		yield return null;
	}
}
