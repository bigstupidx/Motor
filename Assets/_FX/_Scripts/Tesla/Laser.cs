//
// Laser.cs
//
// Author:
// [Yangbowen]
//
// Copyright (C) 2015 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using System.Collections.Generic;
using UnityEngine;

namespace Game {

    public class Laser : MonoBehaviour {

        public List<LineRenderer> LineRenderers;
	    public Transform Pos0;
	    public Transform Pos1;
	    public float DelayTime = 0.3f;

		public void SetPosition(int index,Vector3 pos) {
            for (int i = 0; i < LineRenderers.Count; i++) {
                LineRenderers[i].SetPosition(index,pos);
            }
        }

	    private void OnEnable()
	    {
		    this.DelayInvoke(() =>
		    {
				SetPosition(0, Pos0.position);
				SetPosition(1, Pos1.position);
			}, DelayTime);
			
		}

	    private void OnDisable() {
            for (int i = 0; i < LineRenderers.Count; i++) {
                LineRenderers[i].SetPosition(0, Vector3.zero);
            }
            for (int i = 0; i < LineRenderers.Count; i++) {
                LineRenderers[i].SetPosition(1, Vector3.zero);
            }
        }
    }
}


